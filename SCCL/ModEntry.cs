using Entoarox.Framework;
using Entoarox.Framework.Extensions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using TehPers.Stardew.SCCL.API;

namespace TehPers.Stardew.SCCL {
    public class ModEntry : Mod {
        internal static ModEntry INSTANCE;

        public List<ModEvent> Events = new List<ModEvent>();
        public ContentMerger merger;
        public ModConfig config;

        // SCCL Content folder loading
        public string contentPath;
        public ContentManager modContent;

        private List<Type> injectedTypes = new List<Type>();
        private bool loaded = false;

        public ModEntry() {
            if (INSTANCE == null) INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();
            if (!config.ModEnabled) return;

            this.contentPath = Path.Combine(this.Helper.DirectoryPath, "Content");
            if (!Directory.Exists(this.contentPath)) {
                try {
                    this.Monitor.Log("Creating directory " + this.contentPath);
                    Directory.CreateDirectory(this.contentPath);
                } catch (Exception ex) {
                    this.Monitor.Log("Could not create directory " + this.contentPath + "! Please create it yourself.", LogLevel.Error);
                    this.Monitor.Log(ex.Message, LogLevel.Error);
                    return;
                }
            }

            GameEvents.UpdateTick += UpdateTick;
            LocationEvents.CurrentLocationChanged += CurrentLocationChanged;
            GraphicsEvents.OnPreRenderEvent += OnPreRenderEvent;

            // Testing NBT

            // Writing
            /*NBTTagCompound tag = new NBTTagCompound();
            tag.Set("Name", "Test");
            tag.Set("Success", (byte) 1);

            using (FileStream stream = new FileStream(Path.Combine(Helper.DirectoryPath, "test.dat"), FileMode.OpenOrCreate, FileAccess.Write)) {
                NBTBase.WriteStream(stream, tag);
            }

            // Reading
            using (FileStream stream = new FileStream(Path.Combine(Helper.DirectoryPath, "test.dat"), FileMode.Open, FileAccess.Read)) {
                tag = (NBTBase.ReadStream(stream) as NBTTagCompound) ?? null;
            }*/
        }

        #region XNB Content Registering
        private void registerHandlers() {
            this.Monitor.Log("Loading delegates");
            this.merger = this.merger ?? new ContentMerger();
            this.modContent = this.modContent ?? new ContentManager(Game1.content.ServiceProvider, this.contentPath);

            // Get all xnbs that need delegates
            foreach (string mod in Directory.GetDirectories(Path.Combine(Helper.DirectoryPath, "Content"))) {
                ContentInjector injector = ContentAPI.GetInjector(Path.GetFileName(mod));

                List<string> checkDirs = Directory.GetDirectories(mod).ToList();
                while (checkDirs.Count > 0) {
                    string dir = checkDirs[0];
                    checkDirs.RemoveAt(0);
                    checkDirs.AddRange(Directory.GetDirectories(dir));

                    // Go through each xnb file
                    string[] curList = Directory.GetFiles(dir, "*.xnb");
                    foreach (string xnb in curList) {
                        try {
                            string localModPath = getModRelativePath(mod, xnb);
                            localModPath = localModPath.Substring(0, localModPath.Length - 4);
                            object modAsset = this.modContent.Load<object>(localModPath);

                            if (modAsset != null)
                                injector.RegisterAsset(localModPath.Substring(localModPath.IndexOf('\\') + 1), modAsset);
                        } catch (Exception ex) {
                            this.Monitor.LogOnce("Unable to load " + xnb, LogLevel.Warn, ex);
                        }
                    }
                }
            }

            Command.RegisterCommand("SCCLEnable", "Enables a content injector | SCCLEnable <injector>", new string[] { "(String)<injector>" }).CommandFired += InjectorEnabled;
            Command.RegisterCommand("SCCLDisable", "Disables a content injector | SCCLEnable <injector>", new string[] { "(String)<injector>" }).CommandFired += InjectorDisabled;
            Command.RegisterCommand("SCCLList", "Lists all content injectors").CommandFired += (sender, e) => {
                this.Monitor.Log("Registered injectors: " + string.Join(", ", ContentAPI.GetAllInjectors()), LogLevel.Info);
            };
        }

        public string getModRelativePath(string modPath, string assetName) {
            Uri modUri = new Uri(Path.Combine(this.contentPath, modPath));
            Uri fileUri = new Uri(Path.Combine(modPath, assetName));
            Uri localUri = modUri.MakeRelativeUri(fileUri);
            return WebUtility.UrlDecode(localUri.ToString().Replace('/', '\\'));
        }
        #endregion

        #region Event Handlers
        private void UpdateTick(object sender, EventArgs e) {
            if (this.merger != null) {
                string[] disposedTextures = this.merger.Cache.Where(kv => kv.Value is Texture2D)
                    .Where(kv => (kv.Value as Texture2D).IsDisposed)
                    .Select(kv => kv.Key)
                    .ToArray();

                foreach (string tex in disposedTextures)
                    this.merger.Cache.Remove(tex);
            }

            if (!loaded && !(Game1.content.GetType() == typeof(LocalizedContentManager))) {
                this.loaded = true;
                this.registerHandlers();

                this.Monitor.Log("Loading event injections");
                this.loadEvents();
            }
        }

        private void CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e) {
            GameLocation loc = e.NewLocation;
            if (!Game1.killScreen && Game1.farmEvent == null && loc.currentEvent == null) {
                foreach (ModEvent ev in this.Events) {
                    int? id = ev.getID();
                    if (id == null) continue;
                    if (id < 0) continue;
                    if (ev.Location == loc.name) {
                        this.Monitor.Log(id.ToString(), LogLevel.Trace);
                        if (ev.Repeatable) Game1.player.eventsSeen.Remove((int) id);
                        int eventID = -1;

                        try {
                            eventID = this.Helper.Reflection.GetPrivateMethod(loc, "checkEventPrecondition").Invoke<int>(ev.Condition);
                        } catch {
                            this.Monitor.Log("Failed to check condition for event " + id + "(at '" + loc.name + "')", LogLevel.Error);
                        }

                        if (eventID != -1) {
                            loc.currentEvent = new Event(ev.Data, eventID);

                            if (Game1.player.getMount() != null) {
                                loc.currentEvent.playerWasMounted = true;
                                Game1.player.getMount().dismount();
                            }
                            foreach (NPC character in loc.characters)
                                character.clearTextAboveHead();
                            Game1.eventUp = true;
                            Game1.displayHUD = false;
                            Game1.player.CanMove = false;
                            Game1.player.showNotCarrying();

                            IPrivateField<List<Critter>> crittersF = this.Helper.Reflection.GetPrivateField<List<Critter>>(loc, "critters");
                            List<Critter> critters = crittersF.GetValue();
                            if (critters == null)
                                return;
                            critters.Clear();
                            crittersF.SetValue(critters);
                            break;
                        }
                    }
                }
            }
        }

        private void OnPreRenderEvent(object sender, EventArgs e) {
            if (merger.Dirty.Count > 0) {
                foreach (string asset in merger.Dirty) merger.Cache.Remove(asset);
                merger.Dirty.Clear();
                this.reloadContent();
            }
        }

        private void InjectorEnabled(object sender, EventArgsCommand e) {
            if (e.Command.CalledArgs.Length >= 1) {
                string name = e.Command.CalledArgs[0];
                if (ContentAPI.InjectorExists(name)) {
                    ContentAPI.GetInjector(name).Enabled = true;
                    Monitor.Log(name + " is enabled", LogLevel.Info);
                } else Monitor.Log("No injector with name '" + name + "' exists!", LogLevel.Warn);
            } else Monitor.Log("Injector name was not specified", LogLevel.Warn);
        }

        private void InjectorDisabled(object sender, EventArgsCommand e) {
            if (e.Command.CalledArgs.Length >= 1) {
                string name = e.Command.CalledArgs[0];
                if (ContentAPI.InjectorExists(name)) {
                    ContentAPI.GetInjector(name).Enabled = false;
                    Monitor.Log(name + " is disabled", LogLevel.Info);
                } else Monitor.Log("No injector with name '" + name + "' exists!", LogLevel.Warn);
            } else Monitor.Log("Injector name was not specified", LogLevel.Warn);
        }
        #endregion

        #region Loaders
        private void loadEvents() {
            string path = Path.Combine(this.Helper.DirectoryPath, "Events");

            if (!Directory.Exists(path)) {
                try {
                    this.Monitor.Log("Creating directory " + path);
                    Directory.CreateDirectory(path);
                } catch (Exception ex) {
                    this.Monitor.Log("Could not create directory " + path + "! Please create it yourself.", LogLevel.Error);
                    this.Monitor.Log(ex.Message, LogLevel.Error);
                    return;
                }
            }

            string[] configList = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
            foreach (string configPath in configList) {
                try {
                    EventConfig config = this.Helper.ReadJsonFile<EventConfig>(configPath);
                    if (config != null && config.Enabled) {
                        this.Monitor.Log("Loading " + Path.GetFileName(configPath), LogLevel.Info);
                        this.Events.AddRange(config.Events);
                    }
                    this.Helper.WriteJsonFile(configPath, config);
                } catch (Exception ex) {
                    this.Monitor.Log("Failed to load " + Path.GetFileName(configPath) + ".", LogLevel.Error);
                    this.Monitor.Log(ex.Message, LogLevel.Error);
                    this.Monitor.Log("Maybe your format is invalid?", LogLevel.Warn);
                }
            }
        }

        private void reloadValue<T>(ref T asset, string assetName) {
            object assetObj = asset as object;

            // If the current asset isn't one that was generated by this mod
            if (!this.merger.Cache.ContainsKey(assetName) || this.merger.Cache[assetName] != assetObj) {
                // If the current asset isn't already registered
                if (!ContentAPI.originalInjector.ModContent.ContainsKey(assetName) || ContentAPI.originalInjector.ModContent[assetName].Where(a => a == assetObj).ToArray().Length == 0) {
                    // Register the asset, replacing the old one if it exists
                    this.Monitor.Log("Registering asset to [ORIGINAL]: " + assetName);
                    ContentAPI.originalInjector.ModContent.Remove(assetName);
                    ContentAPI.originalInjector.RefreshAsset(assetName);
                    ContentAPI.originalInjector.RegisterAsset(assetName, asset);
                }
            }
            asset = Game1.content.Load<T>(assetName);
        }

        public void reloadContent() {
            this.merger.Passthrough = true;
            reloadValue(ref Game1.daybg, "LooseSprites\\daybg");
            reloadValue(ref Game1.nightbg, "LooseSprites\\nightbg");
            reloadValue(ref Game1.menuTexture, "Maps\\MenuTiles");
            reloadValue(ref Game1.lantern, "LooseSprites\\Lighting\\lantern");
            reloadValue(ref Game1.windowLight, "LooseSprites\\Lighting\\windowLight");
            reloadValue(ref Game1.sconceLight, "LooseSprites\\Lighting\\sconceLight");
            reloadValue(ref Game1.cauldronLight, "LooseSprites\\Lighting\\greenLight");
            reloadValue(ref Game1.indoorWindowLight, "LooseSprites\\Lighting\\indoorWindowLight");
            reloadValue(ref Game1.shadowTexture, "LooseSprites\\shadow");
            reloadValue(ref Game1.mouseCursors, "LooseSprites\\Cursors");
            reloadValue(ref Game1.animations, "TileSheets\\animations");
            reloadValue(ref Game1.achievements, "Data\\Achievements");
            reloadValue(ref Game1.eventConditions, "Data\\eventConditions");
            reloadValue(ref Game1.NPCGiftTastes, "Data\\NPCGiftTastes");
            reloadValue(ref Game1.dialogueFont, "Fonts\\SpriteFont1");
            reloadValue(ref Game1.smallFont, "Fonts\\SmallFont");
            reloadValue(ref Game1.borderFont, "Fonts\\BorderFont");
            reloadValue(ref Game1.tinyFont, "Fonts\\tinyFont");
            reloadValue(ref Game1.tinyFontBorder, "Fonts\\tinyFontBorder");
            reloadValue(ref Game1.smoothFont, "Fonts\\smoothFont");
            reloadValue(ref Game1.objectSpriteSheet, "Maps\\springobjects");
            reloadValue(ref Game1.toolSpriteSheet, "TileSheets\\tools");
            reloadValue(ref Game1.cropSpriteSheet, "TileSheets\\crops");
            reloadValue(ref Game1.emoteSpriteSheet, "TileSheets\\emotes");
            reloadValue(ref Game1.debrisSpriteSheet, "TileSheets\\debris");
            reloadValue(ref Game1.bigCraftableSpriteSheet, "TileSheets\\Craftables");
            reloadValue(ref Game1.rainTexture, "TileSheets\\rain");
            reloadValue(ref Game1.buffsIcons, "TileSheets\\BuffsIcons");
            reloadValue(ref Game1.objectInformation, "Data\\ObjectInformation");
            reloadValue(ref Game1.bigCraftablesInformation, "Data\\BigCraftablesInformation");
            reloadValue(ref Tool.weaponsTexture, "TileSheets\\weapons");
            reloadValue(ref Game1.getFarm().houseTextures, "Buildings\\houses");
            reloadValue(ref StardewValley.TerrainFeatures.Flooring.floorsTexture, "TerrainFeatures\\Flooring");
            this.merger.Passthrough = false;
            EntoFramework.GetContentRegistry().ReloadStaticReferences(); // Reloading twice is fine since I cache any modded files
        }
        #endregion
    }
}

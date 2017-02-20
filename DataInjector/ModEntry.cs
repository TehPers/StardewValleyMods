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

        internal ContentInjector injector;

        // SCCL Content folder loading
        public string contentPath;
        public ContentManager modContent;

        private List<Type> injectedTypes = new List<Type>();
        private CustomContentManager tmpManager;
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

            if (!loaded && Game1.content != null && Game1.content.GetType() == typeof(LocalizedContentManager)) {
                this.merger = this.merger ?? new ContentMerger();
                tmpManager = tmpManager ?? new CustomContentManager(Game1.content.RootDirectory, Game1.content.ServiceProvider);
                Game1.content = tmpManager;
            } else if (!loaded && !(Game1.content.GetType() == typeof(LocalizedContentManager))) {
                this.loaded = true;
                this.registerHandlers();
                EntoFramework.GetContentRegistry().ReloadStaticReferences();
                this.reloadContent(); // Reloading twice is fine since I cache any changed files

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

        private void reloadContent() {
            Game1.daybg = Game1.content.Load<Texture2D>("LooseSprites\\daybg");
            Game1.nightbg = Game1.content.Load<Texture2D>("LooseSprites\\nightbg");
            Game1.menuTexture = Game1.content.Load<Texture2D>("Maps\\MenuTiles");
            Game1.lantern = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\lantern");
            Game1.windowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\windowLight");
            Game1.sconceLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\sconceLight");
            Game1.cauldronLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\greenLight");
            Game1.indoorWindowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\indoorWindowLight");
            Game1.shadowTexture = Game1.content.Load<Texture2D>("LooseSprites\\shadow");
            Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            Game1.animations = Game1.content.Load<Texture2D>("TileSheets\\animations");
            Game1.achievements = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
            Game1.eventConditions = Game1.content.Load<Dictionary<string, bool>>("Data\\eventConditions");
            Game1.NPCGiftTastes = Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes");
            Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
            Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
            Game1.borderFont = Game1.content.Load<SpriteFont>("Fonts\\BorderFont");
            Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
            Game1.tinyFontBorder = Game1.content.Load<SpriteFont>("Fonts\\tinyFontBorder");
            Game1.smoothFont = Game1.content.Load<SpriteFont>("Fonts\\smoothFont");
            Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
            Game1.toolSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\tools");
            Game1.cropSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\crops");
            Game1.emoteSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\emotes");
            Game1.debrisSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\debris");
            Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables");
            Game1.rainTexture = Game1.content.Load<Texture2D>("TileSheets\\rain");
            Game1.buffsIcons = Game1.content.Load<Texture2D>("TileSheets\\BuffsIcons");
            Game1.objectInformation = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
            Game1.bigCraftablesInformation = Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation");
            Tool.weaponsTexture = Game1.content.Load<Texture2D>("TileSheets\\weapons");
        }
        #endregion
    }
}

using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using TehPers.Stardew.SCCL.API;
using TehPers.Stardew.SCCL.Configs;

namespace TehPers.Stardew.SCCL {
    public class ModEntry : Mod {
        internal static ModEntry INSTANCE;

        public const bool SI_COMPAT = false; // Experimental compatibility with SeasonalImmersion
        public const int MOD_INDEX = 10000;

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

            this.merger = new ContentMerger();

            GameEvents.UpdateTick += UpdateTick;
            LocationEvents.CurrentLocationChanged += CurrentLocationChanged;
            GraphicsEvents.OnPreRenderEvent += OnPreRenderEvent;
            GameEvents.LoadContent += loadFromFolder;

#pragma warning disable
            // Register the asset loader last (hopefully) so that other mods get to fight over the asset first.
            Thread t = new Thread(() => {
                Thread.Sleep(50);
                ContentEvents.AssetLoading += this.merger.AssetLoading;
                });
            t.Start();
#pragma warning restore

            #region Console Commands
            Helper.ConsoleCommands.Add("SCCLEnable", "Enables a content injector | SCCLEnable <injector>", (cmd, args) => {
                if (args.Length >= 1) {
                    string name = args[0];
                    if (ContentAPI.InjectorExists(name)) {
                        ContentAPI.GetInjector(this, name).Enabled = true;
                        Monitor.Log(name + " is enabled", LogLevel.Info);
                    } else Monitor.Log("No injector with name '" + name + "' exists!", LogLevel.Warn);
                } else Monitor.Log("Injector name was not specified", LogLevel.Warn);
            });
            Helper.ConsoleCommands.Add("SCCLDisable", "Disables a content injector | SCCLEnable <injector>", (cmd, args) => {
                if (args.Length >= 1) {
                    string name = args[0];
                    if (ContentAPI.InjectorExists(name)) {
                        ContentAPI.GetInjector(this, name).Enabled = false;
                        Monitor.Log(name + " is disabled", LogLevel.Info);
                    } else Monitor.Log("No injector with name '" + name + "' exists!", LogLevel.Warn);
                } else Monitor.Log("Injector name was not specified", LogLevel.Warn);
            });
            Helper.ConsoleCommands.Add("SCCLList", "Lists all content injectors | SCCLList", (cmd, args) => {
                this.Monitor.Log("Registered injectors: " + string.Join(", ", ContentAPI.GetAllInjectors()), LogLevel.Info);
            });
            Helper.ConsoleCommands.Add("SCCLReload", "Reloads the config (ModEnabled is ignored) | SCCLReload", (cmd, args) => {
                this.config = this.Helper.ReadConfig<ModConfig>();
                // TODO: remove all previously loaded assets
                this.loadFromFolder(this, new EventArgs());
            });
            #endregion
        }

        #region XNB Content Registering
        private void loadFromFolder(object sender, EventArgs e) {
            this.Monitor.Log("Loading delegates", LogLevel.Info);
            this.modContent = this.modContent ?? new ContentManager(Game1.content.ServiceProvider, this.contentPath);

            // Get all xnbs that need delegates
            foreach (string mod in Directory.GetDirectories(Path.Combine(Helper.DirectoryPath, "Content"))) {
                ContentInjector injector = ContentAPI.GetInjector(this, Path.GetFileName(mod));

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
                        } catch (Exception) {
                            this.Monitor.Log("Unable to load " + xnb, LogLevel.Warn);
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
            if (!loaded) {
                this.loaded = true;

                this.Monitor.Log("Loading event injections");
                this.loadEvents();
            }
        }

        private void OnPreRenderEvent(object sender, EventArgs e) {
            this.merger.RefreshAssets();
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
        #endregion
    }
}

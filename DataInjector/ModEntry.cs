using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.DataInjector {
    public class ModEntry : Mod {

        public List<ModEvent> Events = new List<ModEvent>();
        public ContentManager modContent;

        private bool loaded = false;

        public override void Entry(IModHelper helper) {
            GameEvents.UpdateTick += UpdateTick;
            LocationEvents.CurrentLocationChanged += CurrentLocationChanged;
        }

        #region Event Handlers
        private void UpdateTick(object sender, EventArgs e) {
            if (!loaded && Game1.objectInformation != null) {
                modContent = new ContentManager(Game1.content.ServiceProvider, this.Helper.DirectoryPath);
                loaded = true;

                this.Monitor.Log("Loading ObjectInformation.xnb injections");
                loadObjectInfo();

                this.Monitor.Log("Loading event injections");
                loadEvents();
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
                        this.Monitor.Log(id.ToString());
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

        #region Loading
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

        private void loadObjectInfo() {
            string path = Path.Combine(this.Helper.DirectoryPath, "ObjectInformation");

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

            Uri modUri = new Uri(path);
            Dictionary<int, string> original = Game1.objectInformation;
            Dictionary<int, string> diffs = new Dictionary<int, string>();
            string[] fileList = Directory.GetFiles(path, "*.xnb", SearchOption.TopDirectoryOnly);
            foreach (string filePath in fileList) {
                Uri fileUri = new Uri(filePath);
                Uri localUri = modUri.MakeRelativeUri(fileUri);
                string localConfigPath = localUri.ToString().Replace('/', '\\');
                localConfigPath = localConfigPath.Substring(0, localConfigPath.Length - 4);

                try {
                    // TODO: Load the xnb file, then calculate diffs with the original and merge the changes
                    Dictionary<int, string> injections = this.modContent.Load<Dictionary<int, string>>(localConfigPath);
                    this.Monitor.Log(string.Format("Loading {0} ({1} entries)", Path.GetFileName(filePath), injections.Count), LogLevel.Info);

                    bool collision = false;
                    int changes = 0;
                    foreach (KeyValuePair<int, string> injection in injections) {
                        if (!(original.ContainsKey(injection.Key) && original[injection.Key].Equals(injection.Value))) {
                            changes++;
                            if (diffs.ContainsKey(injection.Key)) {
                                if (!collision) {
                                    this.Monitor.Log("Collision detected in object information! This injection might not work as intended.");
                                    collision = true;
                                }
                            } else diffs[injection.Key] = injection.Value;
                        }
                    }

                    this.Monitor.Log(changes + " changes were detected", LogLevel.Info);
                } catch (Exception ex) {
                    this.Monitor.Log("Failed to load " + localConfigPath + ".", LogLevel.Error);
                    this.Monitor.Log(ex.Message, LogLevel.Error);
                }
            }

            foreach (KeyValuePair<int, string> diff in diffs)
                Game1.objectInformation[diff.Key] = diff.Value;
            this.Monitor.Log(string.Format("{0} changes injected into ObjectInformation.xnb", diffs.Count), LogLevel.Info);
        }
        #endregion
    }
}

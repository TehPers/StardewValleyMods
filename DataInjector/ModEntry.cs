using Microsoft.Xna.Framework;
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
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.DataInjector {
    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public List<ModEvent> Events = new List<ModEvent>();
        public DiffContentManager diffContent;
        public ModConfig config;

        private bool loaded = false;

        public ModEntry() {
            INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();

            GameEvents.UpdateTick += UpdateTick;
            LocationEvents.CurrentLocationChanged += CurrentLocationChanged;
        }

        #region Event Handlers
        private void UpdateTick(object sender, EventArgs e) {
            if (!loaded && Game1.content != null) {
                this.loaded = true;

                this.Monitor.Log("Loading content manager");
                this.loadContent();

                this.Monitor.Log("Loading event injections");
                this.loadEvents();
            }

            Dictionary<int, string> o = Game1.objectInformation;

            if (this.diffContent != null && Game1.content != this.diffContent) {
                Game1.content = this.diffContent;
                Game1.temporaryContent = this.diffContent;
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
        private void loadContent() {
            this.diffContent = new DiffContentManager(Game1.content.ServiceProvider, Path.GetFullPath(Path.Combine(this.Helper.DirectoryPath, "..", "..", "Content")), Path.Combine(this.Helper.DirectoryPath, "Content"));
            Game1.content = this.diffContent;
            
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
        }

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

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;

namespace TehPers.FishingOverhaul {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig config;
        private bool postEntryDone = false;

        public ModEntry() {
            ModEntry.INSTANCE = this;
        }

        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper) {
            this.config = helper.ReadConfig<ModConfig>();

            GameEvents.UpdateTick += this.UpdateTick;
            ControlEvents.KeyPressed += this.KeyPressed;
        }

        private void postEntry() {
            this.addObjects();
        }

        #region "Events"
        private void UpdateTick(object sender, EventArgs e) {
            if (!postEntryDone) {
                this.postEntry();
                postEntryDone = true;
            }
            tryChangeFishingTreasure();
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.T)
                Game1.player.addItemToInventory(new StardewValley.Object(Vector2.Zero, 8001, 1));
        }
        #endregion

        private void addObjects() {
            // TODO: No clue how to do this yet...

            //this.Monitor.Log(Game1.objectInformation[Objects.STONE]);
            //Game1.objectInformation[8001] = "Custom Object/9999/-300/Basic -16/A custom item.";
            this.registerObjectInformation(8001, "Custom Object", "A custom item", "Basic", -16, 9999, -300);
            //Game1.objec
        }

        private void tryChangeFishingTreasure() {
            if (Game1.player.CurrentTool is FishingRod) {
                //this.Monitor.Log("Player is holding a fishing rod");
                FishingRod rod = (FishingRod) Game1.player.CurrentTool;
                //IEnumerable<TemporaryAnimatedSprite> anims;

                // Look through all animated sprites in the main game
                foreach (TemporaryAnimatedSprite anim in Game1.screenOverlayTempSprites) {
                    if (anim.endFunction == rod.startMinigameEndFunction) {
                        this.Monitor.Log("Overriding bobber bar", LogLevel.Trace);
                        anim.endFunction = (i => FishingRodOverrides.startMinigameEndFunction(rod, i));
                    }
                }

                // Look through all animated sprites in the fishing rod
                foreach (TemporaryAnimatedSprite anim in rod.animations) {
                    if (anim.endFunction == rod.openTreasureMenuEndFunction) {
                        this.Monitor.Log("Overriding treasure animation end function", LogLevel.Trace);
                        anim.endFunction = (i => FishingRodOverrides.openTreasureMenuEndFunction(rod, i));
                    }
                }
            }
        }

        public void registerObjectInformation(int id, string data) {
            if (Game1.objectInformation.ContainsKey(id))
                this.Monitor.Log("Overriding object information for id: " + id, LogLevel.Warn);
            Game1.objectInformation[id] = data;
        }

        public void registerObjectInformation(int id, string name, string description, string type, int category, int price = -1, int edibility = -300) {
            registerObjectInformation(id, String.Format("{0}/{1}/{2}/{3} {4}/{5}", name, price, edibility, type, category, description));
        }
    }
}
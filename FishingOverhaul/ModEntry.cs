using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;

namespace TehPers.Stardew.FishingOverhaul {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig config;

        public ModEntry() {
            ModEntry.INSTANCE = this;
        }

        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper) {
            this.config = helper.ReadConfig<ModConfig>();
            if (!config.ModEnabled) return;

            GameEvents.UpdateTick += this.UpdateTick;
            //ControlEvents.KeyPressed += this.KeyPressed;
        }

        #region "Events"
        private void UpdateTick(object sender, EventArgs e) {
            tryChangeFishingTreasure();
        }
        #endregion

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
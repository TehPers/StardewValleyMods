using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using TehPers.Stardew.FishingOverhaul.Configs;
using TehPers.Stardew.Framework;
using static TehPers.Stardew.FishingOverhaul.Configs.ConfigFish;

namespace TehPers.Stardew.FishingOverhaul {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ConfigMain config;
        public ConfigTreasure treasureConfig;
        public ConfigFish fishConfig;

        public ModEntry() {
            ModEntry.INSTANCE = this;
        }

        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper) {
            // Load configs
            this.config = helper.ReadConfig<ConfigMain>();
            this.treasureConfig = helper.ReadJsonFile<ConfigTreasure>("treasure.json") ?? new ConfigTreasure();
            this.fishConfig = helper.ReadJsonFile<ConfigFish>("fish.json") ?? new ConfigFish();

            // Make sure the extra configs are generated
            helper.WriteJsonFile<ConfigTreasure>("treasure.json", this.treasureConfig);
            helper.WriteJsonFile<ConfigFish>("fish.json", this.fishConfig);

            // Stop here if the mod is disabled
            if (!config.ModEnabled) return;

            // Events
            GameEvents.UpdateTick += this.UpdateTick;
            ControlEvents.KeyPressed += KeyPressed;

            // DEBUG: Populate global fish data in config
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {
            if (this.fishConfig.PossibleFish == null) {
                // Auto-populate the fish config file if it's empty
                this.fishConfig.populateData();
                this.Helper.WriteJsonFile<ConfigFish>("fish.json", this.fishConfig);
            }

            tryChangeFishingTreasure();
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.R && Game1.player.CurrentTool is FishingRod) {
                FishingRodOverrides.startMinigameEndFunction(Game1.player.CurrentTool as FishingRod, 702);
            }
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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Stardew.FishingOverhaul.Configs;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.FishingOverhaul {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        public const bool DEBUG = false;
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
            helper.WriteJsonFile("treasure.json", this.treasureConfig);
            helper.WriteJsonFile("fish.json", this.fishConfig);

            this.config.AdditionalLootChance = (float) Math.Min(this.config.AdditionalLootChance, 0.99);
            helper.WriteConfig(this.config);

            // Stop here if the mod is disabled
            if (!config.ModEnabled) return;

            // Events
            GameEvents.UpdateTick += this.UpdateTick;
            ControlEvents.KeyPressed += KeyPressed;

            // DEBUG: Populate global fish data in config
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {
            // Auto-populate the fish config file if it's empty
            if (this.fishConfig.PossibleFish == null) {
                this.fishConfig.populateData();
                this.Helper.WriteJsonFile("fish.json", this.fishConfig);
            }

            tryChangeFishingTreasure();

            if (config.IndestructibleTackle && Game1.player.CurrentTool is FishingRod) {
                FishingRod rod = Game1.player.CurrentTool as FishingRod;
                StardewValley.Object bobber = rod.attachments[1];
                if (bobber != null)
                    bobber.scale.Y = 1.0f;
            }
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            Keys getFishKey;
            if (Enum.TryParse(config.GetFishInWaterKey, out getFishKey) && e.KeyPressed == getFishKey) {
                if (Game1.currentLocation != null) {
                    WaterType w = Helpers.convertWaterType(Game1.currentLocation.getFishingLocation(Game1.player.getTileLocation())) ?? WaterType.BOTH;
                    Season s = Helpers.toSeason(Game1.currentSeason) ?? Season.SPRINGSUMMERFALLWINTER;
                    int[] possibleFish = new int[] { };
                    if (config.PossibleFish.ContainsKey(Game1.currentLocation.name)) {
                        if (Game1.currentLocation is MineShaft)
                            possibleFish = config.PossibleFish[Game1.currentLocation.name].Where(data => data.Value.meetsCriteria(w, s, Game1.isRaining ? Weather.RAINY : Weather.SUNNY, Game1.timeOfDay, 5, Game1.player.fishingLevel, (Game1.currentLocation as MineShaft).mineLevel)).Select(data => data.Key).ToArray();
                        else
                            possibleFish = config.PossibleFish[Game1.currentLocation.name].Where(data => data.Value.meetsCriteria(w, s, Game1.isRaining ? Weather.RAINY : Weather.SUNNY, Game1.timeOfDay, 5, Game1.player.fishingLevel)).Select(data => data.Key).ToArray();
                    }
                    Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                    string[] fishByName = possibleFish.Select(id => fish[id].Split('/')[0]).ToArray();
                    if (fishByName.Length > 0)
                        Game1.showGlobalMessage("Fish you can catch right now: " + string.Join<string>(", ", fishByName));
                    else
                        Game1.showGlobalMessage("You can't catch any fish there right now!");
                }
            } else if (DEBUG) {
                if (e.KeyPressed == Keys.R && Game1.player.CurrentTool is FishingRod) {
                    FishingRodOverrides.startMinigameEndFunction(Game1.player.CurrentTool as FishingRod, 702);
                }
            }
        }
        #endregion

        private void tryChangeFishingTreasure() {
            if (Game1.player.CurrentTool is FishingRod) {
                //this.Monitor.Log("Player is holding a fishing rod");
                FishingRod rod = (FishingRod) Game1.player.CurrentTool;
                //IEnumerable<TemporaryAnimatedSprite> anims;

                // Look through all animated sprites in the main game
                if (config.OverrideFishing) {
                    foreach (TemporaryAnimatedSprite anim in Game1.screenOverlayTempSprites) {
                        if (anim.endFunction == rod.startMinigameEndFunction) {
                            this.Monitor.Log("Overriding bobber bar", LogLevel.Trace);
                            anim.endFunction = (i => FishingRodOverrides.startMinigameEndFunction(rod, i));
                        }
                    }
                }

                // Look through all animated sprites in the fishing rod
                if (config.OverrideTreasureLoot) {
                    foreach (TemporaryAnimatedSprite anim in rod.animations) {
                        if (anim.endFunction == rod.openTreasureMenuEndFunction) {
                            this.Monitor.Log("Overriding treasure animation end function", LogLevel.Trace);
                            anim.endFunction = (i => FishingRodOverrides.openTreasureMenuEndFunction(rod, i));
                        }
                    }
                }
            }
        }
    }
}
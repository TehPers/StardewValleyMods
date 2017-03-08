using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TehPers.Stardew.FishingOverhaul.Configs;
using TehPers.Stardew.Framework;
using SObject = StardewValley.Object;

namespace TehPers.Stardew.FishingOverhaul {
    /// <summary>The mod entry point.</summary>
    public class ModFishing : Mod {
        public const bool DEBUG = false;
        public static ModFishing INSTANCE;

        public ConfigMain config;
        public ConfigTreasure treasureConfig;
        public ConfigFish fishConfig;
        public ConfigStrings strings;

        private Dictionary<SObject, float> lastDurability = new Dictionary<SObject, float>();

        public ModFishing() {
            ModFishing.INSTANCE = this;
        }

        /// <summary>Initialize the mod.</summary>
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

            OnLanguageChange(LocalizedContentManager.CurrentLanguageCode);

            // Stop here if the mod is disabled
            if (!config.ModEnabled) return;

            // Events
            GameEvents.UpdateTick += this.UpdateTick;
            ControlEvents.KeyPressed += KeyPressed;
            LocalizedContentManager.OnLanguageChange += OnLanguageChange;
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {
            // Auto-populate the fish config file if it's empty
            if (this.fishConfig.PossibleFish == null) {
                this.fishConfig.populateData();
                this.Helper.WriteJsonFile("fish.json", this.fishConfig);
            }

            TryChangeFishingTreasure();

            if (Game1.player.CurrentTool is FishingRod) {
                FishingRod rod = Game1.player.CurrentTool as FishingRod;
                SObject bobber = rod.attachments[1];
                if (bobber != null) {
                    if (lastDurability.ContainsKey(bobber)) {
                        float last = lastDurability[bobber];
                        bobber.scale.Y = last + (bobber.scale.Y - last) * config.TackleDestroyRate;
                        if (bobber.scale.Y <= 0) {
                            lastDurability.Remove(bobber);
                            rod.attachments[1] = null;
                            try {
                                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"));
                            } catch (Exception) {
                                Game1.showGlobalMessage("Your tackle broke!");
                                this.Monitor.Log("Could not load string for broken tackle", LogLevel.Warn);
                            }
                        } else lastDurability[bobber] = bobber.scale.Y;
                    } else lastDurability[bobber] = bobber.scale.Y;
                }
            }
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (Enum.TryParse(config.GetFishInWaterKey, out Keys getFishKey) && e.KeyPressed == getFishKey) {
                if (Game1.currentLocation != null) {
                    int[] possibleFish;
                    if (Game1.currentLocation is MineShaft)
                        possibleFish = FishHelper.getPossibleFish(5, (Game1.currentLocation as MineShaft).mineLevel).Select(f => f.Key).ToArray();
                    else
                        possibleFish = FishHelper.getPossibleFish(5, -1).Select(f => f.Key).ToArray();
                    Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                    string[] fishByName = (
                        from id in possibleFish
                        let data = fish[id].Split('/')
                        select data.Length > 13 ? data[13] : data[0]
                        ).ToArray();
                    if (fishByName.Length > 0)
                        Game1.showGlobalMessage(string.Format(strings.PossibleFish, string.Join<string>(", ", fishByName)));
                    else
                        Game1.showGlobalMessage(strings.NoPossibleFish);
                }
            }
        }

        private void OnLanguageChange(LocalizedContentManager.LanguageCode code) {
            //Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Translations"));
            this.strings = Helper.ReadJsonFile<ConfigStrings>("Translations/" + Helpers.GetLanguageCode() + ".json") ?? new ConfigStrings();
            Helper.WriteJsonFile("Translations/" + Helpers.GetLanguageCode() + ".json", this.strings);
        }
        #endregion

        private void TryChangeFishingTreasure() {
            if (Game1.player.CurrentTool is FishingRod rod) {
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
                    HashSet<TemporaryAnimatedSprite> toRemove = new HashSet<TemporaryAnimatedSprite>();
                    foreach (TemporaryAnimatedSprite anim in rod.animations) {
                        if (anim.endFunction == rod.openTreasureMenuEndFunction) {
                            this.Monitor.Log("Overriding treasure animation end function", LogLevel.Trace);
                            anim.endFunction = (i => FishingRodOverrides.openTreasureMenuEndFunction(rod, i));
                        } else if (false && anim.endFunction == rod.playerCaughtFishEndFunction) {
#pragma warning disable
                            /*double fishChance = config.FishBaseChance + Game1.player.FishingLevel * config.FishLevelEffect + Game1.dailyLuck * config.FishDailyLuckEffect + Game1.player.LuckLevel * config.FishLuckLevelEffect + FishHelper.getStreak(Game1.player) * config.FishStreakEffect;
                            if (FishHelper.isTrash(anim.extraInfoForEndBehavior) && Game1.random.NextDouble() < config.FishBaseChance) {
                                // Remove the catching animation
                                anim.endFunction = (extra) => { };
                                toRemove.Add(anim);
                                Game1.player.completelyStopAnimatingOrDoingAction();

                                // Undo all the stuff pullFishFromWater does
                                Game1.player.gainExperience(1, -1);

                                // Add the *HIT* animation and whatnot
                                rod.hit = true;
                                List<TemporaryAnimatedSprite> overlayTempSprites = Game1.screenOverlayTempSprites;
                                TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, Helper.Reflection.GetPrivateValue<Vector2>(rod, "bobber") + new Vector2(-140f, (float) (-Game1.tileSize * 5 / 2))), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true);
                                temporaryAnimatedSprite.scaleChangeChange = -0.005f;
                                Vector2 vector2 = new Vector2(0.0f, -0.1f);
                                temporaryAnimatedSprite.motion = vector2;
                                TemporaryAnimatedSprite.endBehavior endBehavior = new TemporaryAnimatedSprite.endBehavior(rod.startMinigameEndFunction);
                                temporaryAnimatedSprite.endFunction = endBehavior;
                                int parentSheetIndex = FishHelper.getRandomFish(Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance")); // This doesn't matter, it gets overridden anyway
                                temporaryAnimatedSprite.extraInfoForEndBehavior = parentSheetIndex;
                                overlayTempSprites.Add(temporaryAnimatedSprite);
                                Game1.playSound("FishHit");
                            }*/
#pragma warning restore
                        }
                    }

                    rod.animations.RemoveAll(anim => toRemove.Contains(anim));
                }
            }
        }

        #region Fish Data Generator
        public void GenerateWeightedFishData(string path) {
            IEnumerable<FishInfo> fishList = (from fishInfo in config.PossibleFish
                                              let loc = fishInfo.Key
                                              from entry in fishInfo.Value
                                              let fish = entry.Key
                                              let data = entry.Value
                                              let seasons = data.Season
                                              let chance = data.Chance
                                              select new FishInfo() { Seasons = seasons, Location = loc, Fish = fish, Chance = chance }
             );

            Dictionary<string, Dictionary<string, Dictionary<int, double>>> result = new Dictionary<string, Dictionary<string, Dictionary<int, double>>>();

            // Spring
            Season s = Season.SPRING;
            string str = "spring";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            IEnumerable<FishInfo> seasonalFish = fishList.Where((info) => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            // Summer
            s = Season.SUMMER;
            str = "summer";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            seasonalFish = fishList.Where((info) => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            // Fall
            s = Season.FALL;
            str = "fall";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            seasonalFish = fishList.Where((info) => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            // Winter
            s = Season.WINTER;
            str = "winter";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            seasonalFish = fishList.Where((info) => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            this.Helper.WriteJsonFile("path", result);
        }

        private class FishInfo {
            public Season Seasons { get; set; }
            public string Location { get; set; }
            public double Chance { get; set; }
            public int Fish { get; set; }
        }
        #endregion
    }
}
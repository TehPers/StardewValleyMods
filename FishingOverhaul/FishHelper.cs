using System;
using System.Collections.Generic;
using System.Linq;
using FishingOverhaul.Configs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using TehCore;
using TehCore.Enums;
using TehCore.Weighted;
using SObject = StardewValley.Object;
using SFarmer = StardewValley.Farmer;

namespace FishingOverhaul {
    public class FishHelper {

        public static int? GetRandomFish(Farmer who, int mineLevel = -1) => FishHelper.GetRandomFish(FishHelper.GetPossibleFish(who));

        public static int? GetRandomFish(IEnumerable<IWeightedElement<int?>> possibleFish) {
            ConfigMain config = ModFishing.Instance.MainConfig;
            possibleFish = possibleFish.ToList();

            // Filter out legendaries
            if (!config.CustomLegendaries)
                possibleFish = possibleFish.Where(e => e.Value != null && !FishHelper.IsLegendary(e.Value.Value));

            // No possible fish
            if (!possibleFish.Any())
                return null;

            // Select a fish
            return possibleFish.Choose(Game1.random);
        }

        public static IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who) {
            Season s = Extensions.ToSeason(Game1.currentSeason) ?? Season.Spring | Season.Summer | Season.Fall | Season.Winter;
            WaterType w = Extensions.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            int mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return FishHelper.GetPossibleFish(who, who.currentLocation?.Name ?? "", w, s, Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        public static IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who, string locationName, WaterType water, Season season, Weather weather, int time, int fishLevel, int mineLevel = -1) {
            // Custom handling for farm maps
            if (locationName == "Farm") {
                switch (Game1.whichFarm) {
                    case 1: {
                            // Forest fish + town fish
                            IEnumerable<IWeightedElement<int?>> forestFish = FishHelper.GetPossibleFish(who, "Forest", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.3);
                            IEnumerable<IWeightedElement<int?>> townFish = FishHelper.GetPossibleFish(who, "Town", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.7);
                            return forestFish.Concat(townFish);
                        }
                    case 2: {
                            // Forest fish + woodskip
                            float scale = 0.05F + (float) Game1.dailyLuck;
                            IEnumerable<IWeightedElement<int?>> forestFish = FishHelper.GetPossibleFish(who, "Forest", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(1 - scale);
                            IWeightedElement<int?>[] woodSkip = { new WeightedElement<int?>(734, scale) };
                            return forestFish.Concat(woodSkip);
                        }
                    case 3: {
                            // Forest fish + default farm fish
                            IEnumerable<IWeightedElement<int?>> forestFish = FishHelper.GetPossibleFish(who, "Forest", water, season, weather, time, fishLevel, mineLevel);
                            IEnumerable<IWeightedElement<int?>> farmFish = FishHelper.GetPossibleFishWithoutFarm(who, locationName, water, season, weather, time, fishLevel, mineLevel);
                            return forestFish.Concat(farmFish);
                        }
                    case 4: {
                            // Mountain fish + default farm fish
                            IEnumerable<IWeightedElement<int?>> forestFish = FishHelper.GetPossibleFish(who, "Mountain", water, season, weather, fishLevel, mineLevel).NormalizeTo(0.35);
                            IEnumerable<IWeightedElement<int?>> farmFish = FishHelper.GetPossibleFishWithoutFarm(who, locationName, water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                            return forestFish.Concat(farmFish);
                        }
                }
            }

            return FishHelper.GetPossibleFishWithoutFarm(who, locationName, water, season, weather, time, fishLevel, mineLevel);
        }

        private static IEnumerable<IWeightedElement<int?>> GetPossibleFishWithoutFarm(Farmer who, string locationName, WaterType water, Season season, Weather weather, int time, int fishLevel, int mineLevel = -1) {
            // Check if this location has fish data
            if (!ModFishing.Instance.FishConfig.PossibleFish.ContainsKey(locationName))
                return new[] { new WeightedElement<int?>(null, 1) };

            // Check if this is the farm
            if (locationName == "Farm" && !ModFishing.Instance.MainConfig.GlobalFishSettings.AllowFishOnAllFarms)
                return new[] { new WeightedElement<int?>(null, 1) };

            // Get chance for fish
            float fishChance = FishHelper.GetFishChance(who);

            // Filter all the fish that can be caught at that location
            IEnumerable<IWeightedElement<int?>> fish = ModFishing.Instance.FishConfig.PossibleFish[locationName].Where(f => {
                // Legendary fish criteria
                if (FishHelper.IsLegendary(f.Key)) {
                    // If custom legendaries is disabled, then don't include legendary fish. They are handled in CustomFishingRod
                    if (!ModFishing.Instance.MainConfig.CustomLegendaries) {
                        return false;
                    }

                    // If recatchable legendaries is disabled, then make sure this fish hasn't been caught yet
                    if (!ModFishing.Instance.MainConfig.RecatchableLegendaries && who.fishCaught.ContainsKey(f.Key)) {
                        return false;
                    }
                }

                // Normal criteria check
                return f.Value.MeetsCriteria(water, season, weather, time, fishLevel, mineLevel);
            }).ToWeighted(kv => kv.Value.GetWeightedChance(fishLevel), kv => (int?) kv.Key);

            // Include trash
            IWeightedElement<int?>[] trash = { new WeightedElement<int?>(null, 1) };

            // Combine fish with trash
            return fish.NormalizeTo(fishChance).Concat(trash.NormalizeTo(1 - fishChance));
        }

        public static int GetRandomTrash() => Game1.random.Next(167, 173);

        public static bool IsTrash(int id) => id >= 167 && id <= 172;

        public static bool IsLegendary(int fish) => fish == 159 || fish == 160 || fish == 163 || fish == 682 || fish == 775;

        private static float GetFishChance(SFarmer who) {
            ConfigMain.ConfigGlobalFish config = ModFishing.Instance.MainConfig.GlobalFishSettings;

            // Calculate chance
            float chance = config.FishBaseChance;
            chance += who.FishingLevel * config.FishLevelEffect;
            chance += who.LuckLevel * config.FishLuckLevelEffect;
            chance += (float) Game1.dailyLuck * config.FishDailyLuckEffect;
            chance += FishHelper.GetStreak(who) * config.FishStreakEffect;

            return chance;
        }

        public static float GetTreasureChance(SFarmer who, CustomFishingRod rod) {
            ConfigMain.ConfigGlobalTreasure config = ModFishing.Instance.MainConfig.GlobalTreasureSettings;

            // Calculate chance
            float chance = config.TreasureChance;
            chance += who.LuckLevel * config.TreasureLuckLevelEffect;
            chance += (float) Game1.dailyLuck * config.TreasureDailyLuckEffect;
            chance += config.TreasureStreakEffect * FishHelper.GetStreak(who);
            if (rod.getBaitAttachmentIndex() == 703)
                chance += config.TreasureBaitEffect;
            if (rod.getBobberAttachmentIndex() == 693)
                chance += config.TreasureBobberEffect;
            if (who.professions.Contains(9))
                chance += config.TreasureChance;

            return Math.Min(chance, config.MaxTreasureChance);
        }

        public static float GetUnawareChance(SFarmer who, int fish) {
            if (FishHelper.IsLegendary(fish))
                return 0F;

            ConfigMain.ConfigUnaware config = ModFishing.Instance.MainConfig.UnawareSettings;

            // Calculate chance
            float chance = config.UnawareChance;
            chance += who.LuckLevel * config.UnawareLuckLevelEffect;
            chance += (float) Game1.dailyLuck * config.UnawareDailyLuckEffect;

            return chance;
        }

        public static int GetStreak(SFarmer who) => FishHelper.Streaks.TryGetValue(who, out int streak) ? streak : 0;

        public static void SetStreak(SFarmer who, int streak) => FishHelper.Streaks[who] = streak;

        public static string GetFishName(int id) {
            // Check if fish names have been loaded in yet
            if (!FishHelper.FishNames.Any()) {
                // Get raw content data for fish
                Dictionary<int, string> fishContent = ModFishing.Instance.Helper.Content.Load<Dictionary<int, string>>("Data\\Fish", ContentSource.GameContent);

                // Store all the names
                foreach (KeyValuePair<int, string> fishData in fishContent) {
                    string[] contentData = fishContent[fishData.Key].Split('/');
                    FishHelper.FishNames[fishData.Key] = contentData.Length > 13 ? contentData[13] : contentData[0];
                }
            }

            return FishHelper.FishNames.TryGetValue(id, out string name) ? name : null;
        }

        private static readonly Dictionary<SFarmer, int> Streaks = new Dictionary<SFarmer, int>();
        private static readonly Dictionary<int, string> FishNames = new Dictionary<int, string>();
    }
}

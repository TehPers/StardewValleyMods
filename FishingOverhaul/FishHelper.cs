using System;
using System.Collections.Generic;
using System.Linq;
using FishingOverhaul.Configs;
using StardewModdingAPI;
using StardewValley;
using TehCore;
using TehCore.Enums;
using SFarmer = StardewValley.Farmer;

namespace FishingOverhaul {
    public class FishHelper {

        private const bool NEVER_TRASH = true;

        public static int? GetRandomFish(int mineLevel = -1) => FishHelper.GetRandomFish(FishHelper.GetPossibleFish(mineLevel));

        public static int? GetRandomFish(IEnumerable<KeyValuePair<int, FishData>> possibleFish) {
            ConfigMain config = ModFishing.Instance.MainConfig;
            possibleFish = possibleFish.ToList();

            //if (!config.ConfigLegendaries)
            //    possibleFish = possibleFish.Where(e => !FishHelper.IsLegendary(e.Key));

            return possibleFish.Any() ? possibleFish.Select(e => new KeyValuePair<int, double>(e.Key, e.Value.Chance)).Choose(Game1.random) : (int?) null;
        }

        public static IEnumerable<KeyValuePair<int, FishData>> GetPossibleFish(int mineLevel = -1) {
            Season s = Extensions.ToSeason(Game1.currentSeason) ?? Season.Spring | Season.Summer | Season.Fall | Season.Winter;
            WaterType w = Extensions.ToWaterType(Game1.currentLocation.getFishingLocation(Game1.player.getTileLocation())) ?? WaterType.Both;
            return FishHelper.GetPossibleFish(Game1.currentLocation.Name, w, s, Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        public static IEnumerable<KeyValuePair<int, FishData>> GetPossibleFish(string location, WaterType water, Season s, Weather w, int time, int fishLevel, int mineLevel = -1) {
            switch (location) {
                default:
                    water = WaterType.Both;
                    break;
            }

            if (!ModFishing.Instance.FishConfig.PossibleFish.ContainsKey(location))
                return new KeyValuePair<int, FishData>[] { };

            return ModFishing.Instance.FishConfig.PossibleFish[location].Where(f => f.Value.MeetsCriteria(water, s, w, time, fishLevel, mineLevel));
        }

        public static int GetRandomTrash() => Game1.random.Next(167, 173);

        public static bool IsTrash(int id) => id >= 167 && id <= 172;

        public static bool IsLegendary(int fish) => fish == 159 || fish == 160 || fish == 163 || fish == 682 || fish == 775;

        public static float GetTrashChance(SFarmer who) => 1F - FishHelper.GetFishChance(who);

        public static float GetFishChance(SFarmer who) {
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

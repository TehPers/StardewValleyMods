using System.Collections.Generic;
using System.Linq;
using FishingOverhaul.Configs;
using StardewValley;
using TehCore;
using TehCore.Enums;
using SFarmer = StardewValley.Farmer;

namespace FishingOverhaul {
    public class FishHelper {

        public static int? GetRandomFish(int depth, int mineLevel = -1) => FishHelper.GetRandomFish(FishHelper.GetPossibleFish(depth, mineLevel));

        public static int? GetRandomFish(IEnumerable<KeyValuePair<int, FishData>> possibleFish) {
            ConfigMain config = ModFishing.Instance.MainConfig;
            possibleFish = possibleFish.ToList();

            //if (!config.ConfigLegendaries)
            //    possibleFish = possibleFish.Where(e => !FishHelper.IsLegendary(e.Key));

            return possibleFish.Any() ? possibleFish.Select(e => new KeyValuePair<int, double>(e.Key, e.Value.Chance)).Choose(Game1.random) : (int?) null;
        }

        public static IEnumerable<KeyValuePair<int, FishData>> GetPossibleFish(int depth, int mineLevel = -1) {
            Season s = Extensions.ToSeason(Game1.currentSeason) ?? Season.Spring | Season.Summer | Season.Fall | Season.Winter;
            WaterType w = Extensions.ToWaterType(Game1.currentLocation.getFishingLocation(Game1.player.getTileLocation())) ?? WaterType.Both;
            return FishHelper.GetPossibleFish(Game1.currentLocation.Name, w, s, Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, depth, Game1.player.FishingLevel, mineLevel);
        }

        public static IEnumerable<KeyValuePair<int, FishData>> GetPossibleFish(string location, WaterType water, Season s, Weather w, int time, int depth, int fishLevel, int mineLevel = -1) {
            switch (location) {
                default:
                    water = WaterType.Both;
                    break;
            }

            if (!ModFishing.Instance.FishConfig.PossibleFish.ContainsKey(location))
                return new KeyValuePair<int, FishData>[] { };

            return ModFishing.Instance.FishConfig.PossibleFish[location].Where(f => f.Value.MeetsCriteria(water, s, w, time, depth, fishLevel, mineLevel));
        }

        public static int GetRandomTrash() => Game1.random.Next(167, 173);

        public static bool IsTrash(int id) => id >= 167 && id <= 172;

        public static bool IsLegendary(int fish) => fish == 159 || fish == 160 || fish == 163 || fish == 682 || fish == 775;

        public static int GetStreak(SFarmer who) => FishHelper.Streaks.TryGetValue(who, out int streak) ? streak : 0;

        public static void SetStreak(SFarmer who, int streak) => FishHelper.Streaks[who] = streak;

        public static Dictionary<SFarmer, int> Streaks = new Dictionary<SFarmer, int>();
    }
}

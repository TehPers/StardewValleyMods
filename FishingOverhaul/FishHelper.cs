using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using TehPers.Stardew.Framework;
using static TehPers.Stardew.FishingOverhaul.Configs.ConfigFish;

namespace TehPers.Stardew.FishingOverhaul {
    public class FishHelper {
        public static int getRandomFish(int depth) {
            Season s = Helpers.toSeason(Game1.currentSeason) ?? Season.SPRINGSUMMERFALLWINTER;
            WaterType w = Helpers.convertWaterType(Game1.currentLocation.getFishingLocation(Game1.player.getTileLocation())) ?? WaterType.BOTH;
            return getRandomFish(w, s, Game1.isRaining ? Weather.RAINY : Weather.SUNNY, Game1.timeOfDay, depth, Game1.player.FishingLevel);
        }

        public static int getRandomFish(WaterType water, Season s, Weather w, int time, int depth, int fishLevel) {
            string loc = Game1.currentLocation.name;
            Dictionary<int, FishData> locFish = ModEntry.INSTANCE.config.PossibleFish[loc];

            IEnumerable<KeyValuePair<int, FishData>> tempLocFish;
            if (Game1.currentLocation is MineShaft) {
                tempLocFish = locFish.Where(e => e.Value.meetsCriteria(water, s, w, time, depth, fishLevel, (Game1.currentLocation as MineShaft).mineLevel));
            } else {
                tempLocFish = locFish.Where(e => e.Value.meetsCriteria(water, s, w, time, depth, fishLevel));
            }

            List<KeyValuePair<int, FishData>> locFishList = tempLocFish.ToList();
            locFishList.Sort((a, b) => a.Value.Chance.CompareTo(b.Value.Chance));

            if (locFishList.Count == 0)
                return getRandomTrash();

            int selectedFish;
            List<int> invalidFish = new List<int>();
            for (int i = 0; ; i = ++i % locFishList.Count) {
                KeyValuePair<int, FishData> kv = locFishList[i];
                int id = kv.Key;
                FishData data = kv.Value;

                if (invalidFish.Contains(id)) continue;

                // Check if legendary and can recatch
                if (!ModEntry.INSTANCE.config.RecatchableLegendaries && isLegendary(id) && Game1.player.fishCaught.ContainsKey(id)) {
                    invalidFish.Add(id);
                    if (invalidFish.Count == locFishList.Count)
                        return getRandomTrash();
                    continue;
                }

                if (Game1.random.NextDouble() < data.getWeightedChance(depth, fishLevel)) {
                    selectedFish = id;
                    break;
                }
            }

            return selectedFish;
        }

        public static int getRandomTrash() {
            return Game1.random.Next(167, 173);
        }

        public static bool isLegendary(int fish) {
            return fish == 159 || fish == 160 || fish == 163 || fish == 682 || fish == 775;
        }

        public static int getStreak(Farmer who) {
            return streaks.ContainsKey(who) ? streaks[who] : 0;
        }

        public static void setStreak(Farmer who, int streak) {
            streaks[who] = streak;
        }

        public static Dictionary<Farmer, int> streaks = new Dictionary<Farmer, int>();
    }
}

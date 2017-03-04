using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using TehPers.Stardew.Framework;
using SFarmer = StardewValley.Farmer;
using static TehPers.Stardew.FishingOverhaul.Configs.ConfigFish;
using TehPers.Stardew.FishingOverhaul.Configs;

namespace TehPers.Stardew.FishingOverhaul {
    public class FishHelper {
        public static int getRandomFish(int depth) {
            Season s = Helpers.toSeason(Game1.currentSeason) ?? Season.SPRINGSUMMERFALLWINTER;
            WaterType w = Helpers.convertWaterType(Game1.currentLocation.getFishingLocation(Game1.player.getTileLocation())) ?? WaterType.BOTH;
            return getRandomFish(w, s, Game1.isRaining ? Weather.RAINY : Weather.SUNNY, Game1.timeOfDay, depth, Game1.player.FishingLevel);
        }

        public static int getRandomFish(WaterType water, Season s, Weather w, int time, int depth, int fishLevel) {
            ConfigMain config = ModFishing.INSTANCE.config;

            string loc = Game1.currentLocation.name;
            Dictionary<int, FishData> locFish = config.PossibleFish[loc];

            IEnumerable<KeyValuePair<int, FishData>> tempLocFish;
            if (Game1.currentLocation is MineShaft) {
                tempLocFish = locFish.Where(e => e.Value.meetsCriteria(water, s, w, time, depth, fishLevel, (Game1.currentLocation as MineShaft).mineLevel));
            } else {
                tempLocFish = locFish.Where(e => e.Value.meetsCriteria(water, s, w, time, depth, fishLevel));
            }

            if (!config.OverrideLegendaries)
                tempLocFish = tempLocFish.Where(e => !isLegendary(e.Key));

            if (tempLocFish.Count() == 0) return getRandomTrash();
            return tempLocFish.Select(e => new KeyValuePair<int, double>(e.Key, e.Value.Chance)).Choose(Game1.random);
        }

        public static int getRandomTrash() {
            return Game1.random.Next(167, 173);
        }

        public static bool isTrash(int id) {
            return id >= 167 && id <= 172;
        }

        public static bool isLegendary(int fish) {
            return fish == 159 || fish == 160 || fish == 163 || fish == 682 || fish == 775;
        }

        public static int getStreak(SFarmer who) {
            return streaks.ContainsKey(who) ? streaks[who] : 0;
        }

        public static void setStreak(SFarmer who, int streak) {
            streaks[who] = streak;
        }

        public static Dictionary<SFarmer, int> streaks = new Dictionary<SFarmer, int>();
    }
}

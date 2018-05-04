using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using TehCore.Enums;
using TehCore;
using StardewModdingAPI;
using TehCore.Configs;

namespace FishingOverhaul.Configs {

    [JsonDescribe]
    public class ConfigFish {

        [Description("All the fish that can be caught")]
        public Dictionary<string, Dictionary<int, FishData>> PossibleFish { get; set; }

        public void PopulateData() {
            ModFishing.Instance.Monitor.Log("Automatically populating fish.json with data from Fish.xnb and Locations.xnb", LogLevel.Info);
            ModFishing.Instance.Monitor.Log("NOTE: If either of these files are modded, the config will reflect the changes! However, legendary fish and fish in the UndergroundMine are not being pulled from those files due to technical reasons.", LogLevel.Info);

            Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            Dictionary<string, string> locations = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

            this.PossibleFish = this.PossibleFish ?? new Dictionary<string, Dictionary<int, FishData>>();

            foreach (KeyValuePair<string, string> locationKv in locations) {
                string location = locationKv.Key;
                string[] locData = locationKv.Value.Split('/');
                const int offset = 4;

                Dictionary<int, FishData> possibleFish = this.PossibleFish.ContainsKey(location) ? this.PossibleFish[location] : new Dictionary<int, FishData>();
                this.PossibleFish[location] = possibleFish;

                for (int i = 0; i <= 3; i++) {
                    Season s = Season.Spring | Season.Summer | Season.Fall | Season.Winter;
                    switch (i) {
                        case 0:
                            s = Season.Spring;
                            break;
                        case 1:
                            s = Season.Summer;
                            break;
                        case 2:
                            s = Season.Fall;
                            break;
                        case 3:
                            s = Season.Winter;
                            break;
                    }

                    string[] seasonData = locData[offset + i].Split(' ');
                    for (int j = 0; j < seasonData.Length; j += 2) {
                        if (seasonData.Length <= j + 1)
                            break;

                        int id = Convert.ToInt32(seasonData[j]);

                        // From location data
                        WaterType water = Extensions.ToWaterType(Convert.ToInt32(seasonData[j + 1])) ?? WaterType.Both;

                        // From fish data
                        if (possibleFish.TryGetValue(id, out FishData f)) {
                            f.WaterType |= water;
                            f.Season |= s;
                        } else if (fish.ContainsKey(id)) {
                            string[] fishInfo = fish[id].Split('/');
                            if (fishInfo[1] == "5") // Junk item
                                continue;

                            string[] times = fishInfo[5].Split(' ');
                            string weather = fishInfo[7].ToLower();
                            int minLevel = Convert.ToInt32(fishInfo[12]);
                            double chance = Convert.ToDouble(fishInfo[10]);

                            Weather w = Weather.Rainy | Weather.Sunny;
                            switch (weather) {
                                case "sunny":
                                    w = Weather.Sunny;
                                    break;
                                case "rainy":
                                    w = Weather.Rainy;
                                    break;
                            }

                            // Add initial data
                            f = new FishData(chance, water, s, Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), minLevel, w);

                            // Add extra time ranges to the data
                            for (int startTime = 2; startTime + 1 < times.Length; startTime += 2)
                                f.Times.Add(new FishData.TimeInterval(Convert.ToInt32(times[startTime]), Convert.ToInt32(times[startTime + 1])));

                            possibleFish[id] = f;
                        } else {
                            ModFishing.Instance.Monitor.Log("A fish listed in Locations.xnb cannot be found in Fish.xnb! Make sure those files aren't corrupt. ID: " + id, LogLevel.Warn);
                        }
                    }
                }
            }

            // NOW THEN, for the special cases >_>

            // Glacierfish
            this.PossibleFish["Forest"][775] = new FishData(.02, WaterType.River, Season.Winter, maxTime: 2000, minLevel: 6);

            // Crimsonfish
            this.PossibleFish["Beach"][159] = new FishData(.02, WaterType.Both, Season.Summer, maxTime: 2000, minLevel: 5);

            // Legend
            this.PossibleFish["Mountain"][163] = new FishData(.02, WaterType.Lake, Season.Spring, maxTime: 2300, minLevel: 10, weather: Weather.Rainy);

            // Angler
            this.PossibleFish["Town"][160] = new FishData(.02, WaterType.Both, Season.Fall, minLevel: 3);

            // Mutant Carp
            this.PossibleFish["Sewer"][682] = new FishData(.02, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter);

            // UndergroundMine
            double mineBaseChance = 0.3;
            if (this.PossibleFish["UndergroundMine"].TryGetValue(156, out FishData ghostFish))
                mineBaseChance = ghostFish.Chance;
            this.PossibleFish["UndergroundMine"][158] = new FishData(mineBaseChance / 3d, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 0);
            this.PossibleFish["UndergroundMine"][158] = new FishData(mineBaseChance / 2d, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 20);
            this.PossibleFish["UndergroundMine"][161] = new FishData(mineBaseChance / 3d, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 60);
            this.PossibleFish["UndergroundMine"][162] = new FishData(mineBaseChance / 3d, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 100);
        }
    }
}

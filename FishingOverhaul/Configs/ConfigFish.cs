using StardewValley;
using System;
using System.Collections.Generic;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.FishingOverhaul.Configs {
    public class ConfigFish {

        public Dictionary<string, Dictionary<int, FishData>> PossibleFish { get; set; } /*= new Dictionary<string, FishData[]>() {
            { "Farm", new FishData[] {

            } }, { "UndergroundMine", new FishData[] {

            } }, { "Desert", new FishData[] {

            } }, { "BusStop", new FishData[] {

            } }, { "Forest", new FishData[] {
                new FishData(145, .45, WaterType.RIVER, Season.SPRINGSUMMER, maxTime: 1900, weather: Weather.SUNNY),
                new FishData(144, .4, WaterType.BOTH, Season.SUMMERWINTER, minDepth: 3),
                new FishData(138, .35, WaterType.RIVER, Season.SUMMER, maxTime: 1900, minDepth: 2, weather: Weather.SUNNY),
                new FishData(132, .45, WaterType.RIVER, Season.SPRINGSUMMERFALLWINTER, minTime: 1800),
                new FishData(706, .35, WaterType.RIVER, Season.SPRINGSUMMERFALL, minTime: 900, minDepth: 2, weather: Weather.RAINY),
                new FishData(704, .15, WaterType.RIVER, Season.SUMMER, maxTime: 1900, minDepth: 3),
                new FishData(702, .45, WaterType.RIVER, Season.SPRINGSUMMERFALLWINTER),

                new FishData(143, .4, WaterType.RIVER, Season.SPRINGFALLWINTER, maxTime: 2400, weather: Weather.RAINY, minDepth: 4),
                new FishData(137, .45, WaterType.LAKE, Season.SPRINGFALL),

                new FishData(140, .4, WaterType.BOTH, Season.FALL, minTime: 1200, weather: Weather.RAINY, minDepth: 2),
                new FishData(139, .4, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3),
                new FishData(699, .2, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3),

                // 699 0 143 0 153 -1 144 -1 141 -1 140 -1 132 0 707 0 702 0
                new FishData(699, .4, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3),
                new FishData(699, .4, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3),
                new FishData(699, .4, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3),
                new FishData(699, .4, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3),
                new FishData(699, .4, WaterType.RIVER, Season.FALL, maxTime: 1900, minDepth: 3)
            } }, { "Town", new FishData[] {} },
            { "Mountain", new FishData[] {} },
            { "Backwoods", new FishData[] {} },
            { "Railroad", new FishData[] {} },
            { "Beach", new FishData[] {} },
            { "Woods", new FishData[] {} },
            { "Sewer", new FishData[] {} },
            { "BugLand", new FishData[] {} },
            { "WitchSwamp", new FishData[] {} },
            { "fishingGame", new FishData[] {} },
            { "Temp", new FishData[] {} }
        };*/

        private Dictionary<int, FishData> globalFishData { get; set; } = new Dictionary<int, FishData>();
        internal List<int> RandomJunk = new List<int>();

        public void populateData() {
            ModEntry.INSTANCE.Monitor.Log("Automatically populating fish.json with data from Fish.xnb and Locations.xnb", StardewModdingAPI.LogLevel.Info);
            ModEntry.INSTANCE.Monitor.Log("NOTE: If either of these files are modded, the config will reflect the changes! However, legendary fish and fish in the UndergroundMine are not being pulled from those files due to technical reasons.", StardewModdingAPI.LogLevel.Info);

            Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            Dictionary<string, string> locations = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

            this.PossibleFish = this.PossibleFish ?? new Dictionary<string, Dictionary<int, FishData>>();

            foreach (KeyValuePair<string, string> locationKV in locations) {
                string location = locationKV.Key;
                string[] locData = locationKV.Value.Split('/');
                const int offset = 4;

                for (int i = 0; i <= 3; i++) {
                    string[] seasonData = locData[offset + i].Split(' ');
                    for (int j = 0; j < seasonData.Length; j += 2) {
                        if (seasonData.Length <= j + 1)
                            break;

                        int id = Convert.ToInt32(seasonData[j]);

                        // From location data
                        WaterType water = WaterType.BOTH;
                        switch (Convert.ToInt32(seasonData[j + 1])) {
                            case 0:
                                water = WaterType.RIVER;
                                break;
                            case 1:
                                water = WaterType.LAKE;
                                break;
                        }

                        Season s = Season.SPRINGSUMMERFALLWINTER;
                        switch (i) {
                            case 0:
                                s = Season.SPRING;
                                break;
                            case 1:
                                s = Season.SUMMER;
                                break;
                            case 2:
                                s = Season.FALL;
                                break;
                            case 3:
                                s = Season.WINTER;
                                break;
                        }

                        // From fish data

                        FishData f;
                        if (this.globalFishData.ContainsKey(id)) {
                            f = this.globalFishData[id];
                            f.WaterType |= water;
                            f.Season |= s;
                        } else {
                            string[] fishInfo = fish[id].Split('/');
                            if (fishInfo[1] == "5") { // Junk item
                                this.RandomJunk.Add(id);
                                continue;
                            }
                            string[] times = fishInfo[5].Split(' ');
                            string weather = fishInfo[7].ToLower();
                            int minDepth = Convert.ToInt32(fishInfo[9]);
                            int minLevel = Convert.ToInt32(fishInfo[12]);
                            double chance = Convert.ToDouble(fishInfo[10]);

                            Weather w = Weather.BOTH;
                            switch (weather) {
                                case "sunny":
                                    w = Weather.SUNNY;
                                    break;
                                case "rainy":
                                    w = Weather.RAINY;
                                    break;
                            }

                            f = new FishData(chance, water, s, Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), minDepth, minLevel, w);
                            this.globalFishData[id] = f;
                        }

                        Dictionary<int, FishData> possibleFish = this.PossibleFish.ContainsKey(location) ? this.PossibleFish[location] : new Dictionary<int, FishData> { };
                        possibleFish[id] = f;
                        this.PossibleFish[location] = possibleFish;
                    }
                }
            }

            // NOW THEN, for the special cases >_>

            // Glacierfish
            this.PossibleFish["Forest"][775] = new FishData(.02, WaterType.RIVER, Season.WINTER, maxTime: 2000, minDepth: 5, minLevel: 6);

            // Crimsonfish
            this.PossibleFish["Beach"][159] = new FishData(.02, WaterType.BOTH, Season.SUMMER, maxTime: 2000, minDepth: 4, minLevel: 5);

            // Legend
            this.PossibleFish["Mountain"][163] = new FishData(.02, WaterType.LAKE, Season.SPRING, maxTime: 2300, minDepth: 5, minLevel: 10, weather: Weather.RAINY);

            // Angler
            this.PossibleFish["Town"][160] = new FishData(.02, WaterType.BOTH, Season.FALL, minDepth: 4, minLevel: 3);

            // Mutant Carp
            this.PossibleFish["Sewer"][682] = new FishData(.02, WaterType.BOTH, Season.FALL, minDepth: 5);

            // UndergroundMine
            // TODO: Fill this in (look at Locations.MineShaft.GetFish)
        }

        public class FishData {
            //public string Name { get; set; }
            public double Chance { get; set; }
            public int MinDepth { get; set; }
            public WaterType WaterType { get; set; }
            public int MinTime { get; set; }
            public int MaxTime { get; set; }
            public Season Season { get; set; }
            public int MinLevel { get; set; }
            public Weather Weather { get; set; }

            public FishData(double chance, WaterType waterType, Season season, int minTime = 600, int maxTime = 2600, int minDepth = 0, int minLevel = 0, Weather weather = Weather.BOTH) {
                this.Chance = chance;
                this.WaterType = waterType;
                this.Season = season;
                this.MinTime = minTime;
                this.MaxTime = maxTime;
                this.MinDepth = minDepth;
                this.MinLevel = minLevel;
                this.Weather = weather;
            }

            public bool meetsCriteria(WaterType waterType, Season season, Weather weather, int time, int depth) {
                return (this.WaterType & waterType) > 0 && (this.Season & season) > 0 && (this.Weather & weather) > 0 && this.MinTime <= time && this.MaxTime >= time && depth >= this.MinDepth;
            }

            public float getWeightedChance(int depth, int level) {
                if (this.MinDepth >= 5) return (float) this.Chance + level / 50f;
                return (float) (5 - depth) / (5 - this.MinDepth) * (float) this.Chance + level / 50f;
            }

            public override string ToString() {
                return string.Format("Chance: {1}, Weather: {2}, Season: {3}", Chance.ToString(), Weather.ToString(), Season.ToString());
            }
        }
    }
}

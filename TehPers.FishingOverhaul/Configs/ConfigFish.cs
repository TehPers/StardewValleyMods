using System;
using System.Collections.Generic;
using System.ComponentModel;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.Core.Helpers.Static;

namespace TehPers.FishingOverhaul.Configs
{
    public class ConfigFish
    {
        [Description("All the fish that can be caught")]
        public Dictionary<string, Dictionary<int, FishData>> PossibleFish { get; set; }

        public void PopulateData()
        {
            ModEntry.Instance.Monitor.Log(
                "Automatically populating fish.json with data from Fish.xnb and Locations.xnb", LogLevel.Info);
            ModEntry.Instance.Monitor.Log(
                "NOTE: If either of these files are modded, the config will reflect the changes! However, legendary fish and fish in the UndergroundMine are not being pulled from those files due to technical reasons.",
                LogLevel.Info);

            var fish = ModEntry.Instance.Helper.Content.Load<Dictionary<int, string>>(@"Data\Fish.xnb",
                ContentSource.GameContent);
            var locations =
                ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>(@"Data\Locations.xnb",
                    ContentSource.GameContent);

            this.PossibleFish ??= new Dictionary<string, Dictionary<int, FishData>>();

            // Loop through each location
            foreach (var locationKv in locations)
            {
                var location = locationKv.Key;
                var locData = locationKv.Value.Split('/');
                const int offset = 4;

                // Create a dictionary of all fish data for this location
                var possibleFish = this.PossibleFish.ContainsKey(location)
                    ? this.PossibleFish[location]
                    : new Dictionary<int, FishData>();
                this.PossibleFish[location] = possibleFish;

                // Loop through each season (order matters)
                for (var i = 0; i <= 3; i++)
                {
                    Season s;
                    switch (i)
                    {
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
                        default:
                            s = Season.All;
                            break;
                    }

                    // Get all the data for this season in this location
                    var seasonData = locData[offset + i].Split(' ');
                    for (var j = 0; j < seasonData.Length; j += 2)
                    {
                        // If the data is invalid, then don't try to read it
                        if (seasonData.Length <= j + 1) break;

                        // Get the ID of this fish
                        var id = Convert.ToInt32(seasonData[j]);

                        // From location data
                        var water = SDVHelpers.ToWaterType(Convert.ToInt32(seasonData[j + 1])) ?? WaterType.Both;

                        // Make sure this is a fish that has data in fish.xnb
                        if (fish.ContainsKey(id))
                        {
                            var fishInfo = fish[id].Split('/');
                            if (fishInfo[1] == "5") // Junk item
                                continue;

                            // Get info about
                            var times = fishInfo[5].Split(' ');
                            var weather = fishInfo[7].ToLower();
                            var minLevel = Convert.ToInt32(fishInfo[12]);
                            var chance = Convert.ToDouble(fishInfo[10]);

                            var w = weather switch
                            {
                                "sunny" => Weather.Sunny,
                                "rainy" => Weather.Rainy,
                                _ => Weather.Rainy | Weather.Sunny
                            };

                            // Try to get existing fish data
                            if (possibleFish.TryGetValue(id, out var f))
                            {
                                f.Season |= s;
                                f.Weather |= w;
                            }
                            else
                            {
                                // Add initial data
                                f = new FishData(chance, null, water, s, minLevel, w);
                                possibleFish[id] = f;
                            }

                            // Add time ranges to the data (duplicates are removed automatically)
                            for (var timeI = 0; timeI + 1 < times.Length; timeI += 2)
                                f.Times.Add(new FishData.TimeInterval(Convert.ToInt32(times[timeI]),
                                    Convert.ToInt32(times[timeI + 1])));
                        }
                        else
                        {
                            ModEntry.Instance.Monitor.Log(
                                "A fish listed in Locations.xnb cannot be found in Fish.xnb! Make sure those files aren't corrupt. ID: " +
                                id, LogLevel.Warn);
                        }
                    }
                }
            }

            // NOW THEN, for the special cases >_>

            // Glacierfish
            this.PossibleFish["Forest"][775] = new FishData(.02, 600, 2000, WaterType.River, Season.Winter, 6);

            // Crimsonfish
            this.PossibleFish["Beach"][159] = new FishData(.02, 600, 2000, WaterType.Both, Season.Summer, 5);

            // Legend
            this.PossibleFish["Mountain"][163] =
                new FishData(.02, 600, 2300, WaterType.Lake, Season.Spring, 10, Weather.Rainy);

            // Angler
            this.PossibleFish["Town"][160] = new FishData(.02, 600, 2600, WaterType.Both, Season.Fall, 3);

            // Mutant Carp
            this.PossibleFish["Sewer"][682] = new FishData(.02, 600, 2600, WaterType.Both,
                Season.Spring | Season.Summer | Season.Fall | Season.Winter);
            if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
            {
                // Glacierfish Jr
                this.PossibleFish["Forest"][902] = new FishData(.02, 600, 2000, WaterType.River, Season.Winter, 6);

                // Legend II
                this.PossibleFish["Mountain"][900] =
                    new FishData(.02, 600, 2300, WaterType.Lake, Season.Spring, 10, Weather.Rainy);

                // Ms Angler
                this.PossibleFish["Town"][899] = new FishData(.02, 600, 2600, WaterType.Both, Season.Fall, 3);

                // Radioactive Carp
                this.PossibleFish["Sewer"][901] = new FishData(.02, 600, 2600, WaterType.Both,
                    Season.Spring | Season.Summer | Season.Fall | Season.Winter);

                // Son of Crimsonfish
                this.PossibleFish["Beach"][898] = new FishData(.02, 600, 2000, WaterType.Both, Season.Summer, 5);
            }

            // UndergroundMine
            var mineBaseChance = 0.3;
            if (this.PossibleFish["UndergroundMine"].TryGetValue(156, out var ghostFish)) mineBaseChance = ghostFish.Chance;
            this.PossibleFish["UndergroundMine"][158] = new FishData(mineBaseChance / 3d, 600, 2600, WaterType.Both,
                Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 0);
            this.PossibleFish["UndergroundMine"][158] = new FishData(mineBaseChance / 2d, 600, 2600, WaterType.Both,
                Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 20);
            this.PossibleFish["UndergroundMine"][161] = new FishData(mineBaseChance / 3d, 600, 2600, WaterType.Both,
                Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 60);
            this.PossibleFish["UndergroundMine"][162] = new FishData(mineBaseChance / 3d, 600, 2600, WaterType.Both,
                Season.Spring | Season.Summer | Season.Fall | Season.Winter, mineLevel: 100);

            // Submarine
            if (!this.PossibleFish.ContainsKey("Submarine")) this.PossibleFish.Add("Submarine", new Dictionary<int, FishData>());
            var curChance = 0.1D;
            this.PossibleFish["Submarine"][800] =
                new FishData(curChance, 600, 2600, WaterType.Both, Season.Winter); // Blobfish
            curChance = (1 - curChance) * 0.18D;
            this.PossibleFish["Submarine"][799] =
                new FishData(curChance, 600, 2600, WaterType.Both, Season.Winter); // Spook Fish
            curChance = (1 - curChance) * 0.28D;
            this.PossibleFish["Submarine"][798] =
                new FishData(curChance, 600, 2600, WaterType.Both, Season.Winter); // Midnight Squid
            curChance = (1 - curChance) * 0.1D;
            this.PossibleFish["Submarine"][154] =
                new FishData(curChance, 600, 2600, WaterType.Both, Season.Winter); // Sea Cucumber
            curChance = (1 - curChance) * 0.08D;
            this.PossibleFish["Submarine"][155] =
                new FishData(curChance, 600, 2600, WaterType.Both, Season.Winter); // Super Cucumber
            curChance = (1 - curChance) * 0.05D;
            this.PossibleFish["Submarine"][149] =
                new FishData(curChance, 600, 2600, WaterType.Both, Season.Winter); // Octupus
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingOverhaul.Api;
using StardewModdingAPI;
using StardewValley;
using TehCore.Api.Enums;
using TehCore.Helpers;
using TehCore.Helpers.Json;

namespace FishingOverhaul.Configs {
    [JsonDescribe]
    public class ConfigFishTraits {
        [Description("The traits for each fish.")]
        public Dictionary<int, FishTraits> FishTraits { get; set; }

        public void PopulateData() {
            ModFishing.Instance.Monitor.Log("Automatically populating fishTraits.json with data from Fish.xnb", LogLevel.Info);
            ModFishing.Instance.Monitor.Log("NOTE: If this file is modded, the config will reflect the changes!", LogLevel.Info);

            Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            this.FishTraits = this.FishTraits ?? new Dictionary<int, FishTraits>();

            // Loop through each fish
            foreach (KeyValuePair<int, string> rawData in fish) {
                try {
                    string[] data = rawData.Value.Split('/');

                    // Get difficulty
                    float difficulty = Convert.ToInt32(data[1]);

                    // Get motion type
                    string motionTypeName = data[2].ToLower();
                    FishMotionType motionType = FishMotionType.MIXED;
                    switch (motionTypeName) {
                        case "mixed":
                            motionType = FishMotionType.MIXED;
                            break;
                        case "dart":
                            motionType = FishMotionType.DART;
                            break;
                        case "smooth":
                            motionType = FishMotionType.SMOOTH;
                            break;
                        case "sinker":
                            motionType = FishMotionType.SINKER;
                            break;
                        case "floater":
                            motionType = FishMotionType.FLOATER;
                            break;
                    }

                    // Get size
                    int minSize = Convert.ToInt32(data[3]);
                    int maxSize = Convert.ToInt32(data[4]);

                    // Add trait
                    this.FishTraits.Add(rawData.Key, new FishTraits {
                        Difficulty = difficulty,
                        MinSize = minSize,
                        MaxSize = maxSize,
                        MotionType = motionType
                    });
                } catch (Exception) {
                    ModFishing.Instance.Monitor.Log($"Failed to generate traits for {rawData.Key}, vanilla traits will be used.", LogLevel.Warn);
                }
            }
        }
    }
}

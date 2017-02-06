using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.FishingOverhaul.Configs {
    public class ConfigMain {
        public bool ModEnabled { get; set; } = true;

        public float CatchSpeed { get; set; } = 1f;
        public float TreasureCatchSpeed { get; set; } = 1.25f;

        public bool RecatchableLegendaries { get; set; } = false;

        public string GetFishInWaterKey { get; set; } = "NumPad9";

        /* Chance for treasure to appear */
        public float TreasureChance { get; set; } = 0.15f;
        public float TreasureBaitEffect { get; set; } = 0.15f;
        public float TreasureBobberEffect { get; set; } = 0.05f;
        public double TreasureLuckLevelEffect { get; set; } = 0.005;
        public double TreasureDailyLuckEffect { get; set; } = 0.5d;
        public float TreasureStreakEffect { get; set; } = 0.02f;

        public float BaseDifficultyMult { get; set; } = 0.85f;
        public float DifficultyStreakEffect { get; set; } = 0.05f;

        /* Chance for fish difficulty to be reduced */
        public float UnawareMult { get; set; } = 0.5f;
        public float UnawareChance { get; set; } = 0.01f;
        public double UnawareLuckLevelEffect { get; set; } = 0.01d;
        public double UnawareDailyLuckEffect { get; set; } = 0.5d;

        /* Perfect streak rewards */
        public int StreakForIncreasedQuality { get; set; } = 3;
        public float PerfectTreasureQualityMult { get; set; } = 5;

        /* Possible treasure from fishing */
        public float AdditionalLootChance { get; set; } = 0.5f;

        // Getters for simplicity
        internal ConfigTreasure.TreasureData[] PossibleLoot {
            get {
                return ModEntry.INSTANCE.treasureConfig.PossibleLoot;
            }
        }

        internal Dictionary<string, Dictionary<int, ConfigFish.FishData>> PossibleFish {
            get {
                return ModEntry.INSTANCE.fishConfig.PossibleFish;
            }
        }
    }
}

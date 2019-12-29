using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Config;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    public class TreasureConfiguration : ITreasureConfiguration
    {
        [Description("Maximum amount of treasure you can find in a single chest while fishing.")]
        public int MaxTreasureQuantity { get; set; } = 3;

        [Description("Whether the treasure randomizer should be allowed to select the same loot option multiple times. Some loot can't be repeated even if duplicate loot is enabled.")]
        public bool AllowDuplicateLoot { get; set; } = true;

        [Description("The chances of obtaining additional loot in your chest. This chance is rolled until either it fails or you get the maximum amount of allowed loot in the chest.")]
        [JsonProperty]
        public FishingChances AdditionalLootChances { get; set; } = new FishingChances
        {
            BaseChance = 0.5f,
            DailyLuckFactor = 0.5f,
            LuckLevelFactor = 0.005f,
            StreakFactor = 0.01f,
        };

        [JsonIgnore]
        IFishingChances ITreasureConfiguration.AdditionalLootChances => this.AdditionalLootChances;
    }
}
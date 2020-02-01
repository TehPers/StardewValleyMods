using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Json;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Private setters are called by the JSON deserializer through reflection.")]
    public class TreasureConfiguration 
    {
        [Description("Maximum amount of treasure you can find in a single chest while fishing.")]
        public int MaxTreasureQuantity { get; private set; } = 3;

        [Description("Whether the treasure randomizer should be allowed to select the same loot option multiple times. Some loot can't be repeated even if duplicate loot is enabled.")]
        public bool AllowDuplicateLoot { get; private set; } = true;

        [Description("The chances of finding treasure while fishing and of obtaining additional loot in your chest. This chance is rolled until either it fails or you get the maximum amount of allowed loot in the chest.")]
        public TreasureChances TreasureChances { get; private set; } = new TreasureChances
        {
            BaseChance = 0.5f,
            DailyLuckFactor = 0.5f,
            LuckLevelFactor = 0.005f,
            StreakFactor = 0.01f,
            MaxChance = 0.5f,
        };
    }
}
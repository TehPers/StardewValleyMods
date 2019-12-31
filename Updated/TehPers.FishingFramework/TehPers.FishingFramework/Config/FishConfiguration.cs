using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Json;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Private setters are called by the JSON deserializer through reflection.")]
    public class FishConfiguration
    {
        [Description("The chance that you'll find a fish instead of trash.")]
        public FishingChances FishChances { get; private set; } = new FishingChances
        {
            BaseChance = 0.5,
            FishingLevelFactor = 0.025,
            LuckLevelFactor =  0.01,
            DailyLuckFactor = 1,
            StreakFactor = 0.005,
            MinChance = 0.1,
            MaxChance = 0.9,
        };

        [Description("Whether this mod affects vanilla legendary fish at all. If false, vanilla legendary fish will be caught in the same places and with the same chances as the vanilla game.")]
        public bool ShouldOverrideVanillaLegendaries { get; private set; } = true;

        [Description("Whether all farm types should have fish. The default farm fish can be defined in fish.json. Vanilla is false")]
        public bool AllowFishOnAllFarms { get; private set; } = false;
    }
}
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api.Config;

namespace TehPers.FishingFramework.Config
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Private setters are used by Newtonsoft.Json during deserialization")]
    [JsonDescribe]
    public class FishingConfiguration : IFishingConfiguration
    {
        [Description("Settings for treasure.")]
        [JsonIgnore]
        ITreasureConfiguration IFishingConfiguration.Treasure => this.Treasure;

        [JsonProperty]
        public TreasureConfiguration Treasure { get; set; } = new TreasureConfiguration();

        [Description("Whether this mod affects vanilla legendary fish at all. If false, vanilla legendary fish will be caught in the same places and with the same chances as the vanilla game.")]
        public bool ShouldOverrideVanillaLegendaries { get; set; } = true;
    }
}

using TehPers.FishingFramework.Api.Config;

namespace TehPers.FishingFramework.Config
{
    internal class GlobalConfiguration : IGlobalConfiguration
    {
        public ITreasureConfiguration Treasure { get; set; }
        public bool ShouldOverrideVanillaLegendaries { get; set; }
    }
}
namespace TehPers.FishingFramework.Api.Config
{
    /// <summary>
    /// The global fishing configuration.
    /// </summary>
    public interface IGlobalConfiguration
    {
        /// <summary>
        /// Gets configuration for treasures found while fishing.
        /// </summary>
        ITreasureConfiguration Treasure { get; }

        /// <summary>
        /// Gets a value indicating whether whether vanilla legendary fish should have overridden catching behavior.
        /// </summary>
        bool ShouldOverrideVanillaLegendaries { get; }
    }
}
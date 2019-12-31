namespace TehPers.FishingFramework.Api.Config
{
    /// <summary>
    /// The configuration for fish. This configuration has no effect on fishing ponds.
    /// </summary>
    public interface IFishConfiguration
    {
        /// <summary>
        /// Gets the chances of finding a fish when hitting something while fishing.
        /// </summary>
        IFishingChances FishChances { get; }

        /// <summary>
        /// Gets a value indicating whether whether vanilla legendary fish should have overridden catching behavior.
        /// </summary>
        bool ShouldOverrideVanillaLegendaries { get; }

        /// <summary>
        /// Gets a value indicating whether whether all farm types should have fish.
        /// </summary>
        bool AllowFishOnAllFarms { get; }
    }
}
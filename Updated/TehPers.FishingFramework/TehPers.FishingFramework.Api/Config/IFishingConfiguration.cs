namespace TehPers.FishingFramework.Api.Config
{
    /// <summary>
    /// The global fishing configuration.
    /// </summary>
    public interface IFishingConfiguration
    {
        /// <summary>
        /// Gets the configuration for fish.
        /// </summary>
        IFishConfiguration Fish { get; }

        /// <summary>
        /// Gets the configuration for treasures found while fishing.
        /// </summary>
        ITreasureConfiguration Treasure { get; }
    }
}
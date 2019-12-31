namespace TehPers.FishingFramework.Api.Config
{
    /// <summary>
    /// The configuration for fishing treasure rewards.
    /// </summary>
    public interface ITreasureConfiguration
    {
        /// <summary>
        /// Gets the maximum number of treasure that can be found while fishing.
        /// </summary>
        int MaxTreasureQuantity { get; }

        /// <summary>
        /// Gets a value indicating whether whether to allow the same treasure reward to appear multiple times in a treasure chest.
        /// </summary>
        bool AllowDuplicateLoot { get; }

        /// <summary>
        /// Gets the chances for treasure to appear while fishing, and the chances for additional loot to appear in a treasure chest. These chances are checked every time an additional item of loot is added to the chest.
        /// </summary>
        ITreasureChances TreasureChances { get; }
    }
}
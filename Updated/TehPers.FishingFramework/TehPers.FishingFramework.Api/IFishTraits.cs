using TehPers.FishingFramework.Api.Minigame;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// The traits of a particular fish.
    /// </summary>
    public interface IFishTraits
    {
        /// <summary>
        /// Gets the item ID for this fish.
        /// </summary>
        int ItemId { get; }

        /// <summary>
        /// Gets a value indicating whether whether this fish is legendary.
        /// </summary>
        bool IsLegendary { get; }

        /// <summary>
        /// Gets the base difficulty of this fish.
        /// </summary>
        float BaseDifficulty { get; }

        /// <summary>
        /// Gets the minimum size of this fish.
        /// </summary>
        float MinSize { get; }

        /// <summary>
        /// Gets the maximum size of this fish.
        /// </summary>
        float MaxSize { get; }

        /// <summary>
        /// Gets this fish's motion controller.
        /// </summary>
        ICatchableEntityController<BobberBarCatchableItem<int>> Controller { get; }
    }
}
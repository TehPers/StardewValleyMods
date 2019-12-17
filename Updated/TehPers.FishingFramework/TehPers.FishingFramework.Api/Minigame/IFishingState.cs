using System.Collections.Generic;

namespace TehPers.FishingFramework.Api.Minigame
{
    /// <summary>
    /// The state of the fishing minigame.
    /// </summary>
    public interface IFishingState
    {
        /// <summary>
        /// Gets the set of catchable fish.
        /// </summary>
        HashSet<BobberBarCatchableItem<int>> Fish { get; }

        /// <summary>
        /// Gets the set of catchable treasure.
        /// </summary>
        HashSet<BobberBarCatchableItem> Treasure { get; }
    }
}
using System.Collections.Generic;
using TehPers.Core.Api;
using TehPers.Core.Api.Weighted;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// Treasure which is sometimes found while fishing.
    /// </summary>
    public interface ITreasureAvailability : IFishingAvailability
    {
        /// <summary>
        /// Gets the possible <see cref="NamespacedId"/>s for this treasure.
        /// </summary>
        IEnumerable<NamespacedId> ItemIds { get; }

        /// <summary>
        /// Gets the minimum quantity of this item that can be found.
        /// </summary>
        int MinQuantity { get; }

        /// <summary>
        /// Gets the maximum quantity of this item that can be found.
        /// </summary>
        int MaxQuantity { get; }

        /// <summary>
        /// Gets a value indicating whether whether to allow this reward to appear multiple times in a treasure chest.
        /// </summary>
        bool AllowDuplicates { get; }
    }
}
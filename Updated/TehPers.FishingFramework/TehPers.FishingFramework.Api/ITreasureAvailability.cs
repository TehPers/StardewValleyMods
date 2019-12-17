using System.Collections.Generic;
using TehPers.Core.Api;
using TehPers.Core.Api.Weighted;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// Treasure which is sometimes found while fishing.
    /// </summary>
    public interface ITreasureAvailability : IFishingAvailability, IWeighted
    {
        /// <summary>
        /// Gets the possible <see cref="NamespacedId"/>s for this treasure.
        /// </summary>
        IEnumerable<NamespacedId> ItemKeys { get; }

        /// <summary>
        /// Gets the minimum quantity of this item that can be found.
        /// </summary>
        int MinimumQuantity { get; }

        /// <summary>
        /// Gets the maximum quantity of this item that can be found.
        /// </summary>
        int MaximumQuantity { get; }
    }
}
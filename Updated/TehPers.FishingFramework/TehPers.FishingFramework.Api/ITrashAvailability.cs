using StardewValley;
using TehPers.Core.Api;
using SObject = StardewValley.Object;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// Trash which is sometimes available while fishing.
    /// </summary>
    public interface ITrashAvailability : IFishingAvailability
    {
        /// <summary>
        /// Gets the <see cref="NamespacedId"/> for this trash.
        /// </summary>
        NamespacedId ItemId { get; }
    }
}
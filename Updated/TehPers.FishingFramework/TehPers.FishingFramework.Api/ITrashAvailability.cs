using TehPers.Core.Api;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// Trash which is sometimes available while fishing.
    /// </summary>
    public interface ITrashAvailability : IFishingAvailability
    {
        /// <summary>
        /// The <see cref="NamespacedId"/> for this trash.
        /// </summary>
        NamespacedId ItemId { get; }
    }
}
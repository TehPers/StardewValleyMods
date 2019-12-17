using TehPers.Core.Api;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// A fish that is sometimes available while fishing.
    /// </summary>
    public interface IFishAvailability : IFishingAvailability
    {
        /// <summary>
        /// Gets the <see cref="NamespacedId"/> for the fish.
        /// </summary>
        NamespacedId FishId { get; }
    }
}
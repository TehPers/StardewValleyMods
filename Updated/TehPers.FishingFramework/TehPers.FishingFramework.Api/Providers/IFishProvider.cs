using System.Collections.Generic;
using TehPers.Core.Api;

namespace TehPers.FishingFramework.Api.Providers
{
    /// <summary>
    /// Provides catchable fish.
    /// </summary>
    public interface IFishProvider
    {
        /// <summary>
        /// Gets all of the custom fish that can be caught while fishing.
        /// </summary>
        IEnumerable<IFishAvailability> Fish { get; }

        /// <summary>
        /// Tries to get the traits for a particular fish. This method must return valid fish traits for any fish listed in <see cref="Fish"/>.
        /// </summary>
        /// <param name="fishId">The fish's item ID.</param>
        /// <param name="traits">The fish's traits.</param>
        /// <returns>Whether fish traits were found.</returns>
        bool TryGetTraits(NamespacedId fishId, out IFishTraits traits);
    }
}

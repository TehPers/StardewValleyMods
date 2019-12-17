using System.Collections.Generic;
using TehPers.Core.Api;

namespace TehPers.FishingFramework.Api.Providers
{
    /// <summary>Provides catchable fish.</summary>
    public interface IFishProvider
    {
        /// <summary>A mapping of fish keys to custom fish traits.</summary>
        IDictionary<NamespacedId, IFishTraits> FishTraits { get; }

        /// <summary>All of the custom fish that can be caught while fishing.</summary>
        IEnumerable<IFishAvailability> Fish { get; }
    }
}

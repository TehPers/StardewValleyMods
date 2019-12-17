using System.Collections.Generic;

namespace TehPers.FishingFramework.Api.Providers
{
    /// <summary>Provides treasure that can be found while fishing.</summary>
    public interface ITreasureProvider
    {
        /// <summary>All of the custom treasure that can be found while fishing.</summary>
        IEnumerable<ITreasureAvailability> Treasure { get; }
    }
}
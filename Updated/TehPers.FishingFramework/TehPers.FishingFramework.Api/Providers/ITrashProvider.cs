using System.Collections.Generic;

namespace TehPers.FishingFramework.Api.Providers
{
    /// <summary>Provides catchable trash.</summary>
    public interface ITrashProvider
    {
        /// <summary>All of the custom trash that can be caught while fishing.</summary>
        IEnumerable<ITrashAvailability> Trash { get; }
    }
}
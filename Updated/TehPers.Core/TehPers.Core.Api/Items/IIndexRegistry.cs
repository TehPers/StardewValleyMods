using System.Collections.Generic;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// A registry for reserving indexes for IDs.
    /// </summary>
    public interface IIndexRegistry
    {
        /// <summary>
        /// Reserves a random index for the given ID.
        /// </summary>
        /// <param name="id">The ID to reserve an index for.</param>
        /// <returns>The index reservation.</returns>
        IIndexReservation Reserve(NamespacedId id);


        /// <summary>
        /// Reserves a specific index for the given ID.
        /// </summary>
        /// <param name="id">The ID to reserve an index for.</param>
        /// <param name="index">The index to reserve.</param>
        /// <returns>The index reservation.</returns>
        IIndexReservation Reserve(NamespacedId id, int index);

        /// <summary>
        /// Gets all assigned indexes.
        /// </summary>
        /// <returns>A mapping of each reserved index.</returns>
        IReadOnlyDictionary<NamespacedId, int> GetAll();
    }
}
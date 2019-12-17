using StardewValley;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Comparer for items, capable of checking if a <see cref="NamespacedId"/> is associated with an item.
    /// </summary>
    public interface IItemComparer
    {
        /// <summary>
        /// Checks if an item is associated with a particular key.
        /// </summary>
        /// <param name="id">The key of the item.</param>
        /// <param name="item">The item to compare against the key.</param>
        /// <returns>True if the item and key are associated, false otherwise.</returns>
        bool IsInstanceOf(in NamespacedId id, Item item);
    }
}
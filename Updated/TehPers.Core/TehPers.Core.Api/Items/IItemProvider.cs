using System;
using StardewValley;
using TehPers.Core.Api.Drawing.Sprites;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Provider for a set of items.
    /// </summary>
    public interface IItemProvider
    {
        /// <summary>
        /// Invalidates any assets used by the items registered by this provider.
        /// This is called after each item is assigned an index, which occurs when a save is loaded or when connecting to a multiplayer game.
        /// </summary>
        void InvalidateAssets();

        /// <summary>
        /// Checks if an item is an instance of a particular key, but cannot guarantee that an item is not an instance of that key. To perform a more exhaustive check, use <see cref="IGlobalItemProvider"/>.
        /// </summary>
        /// <param name="id">The key of the item.</param>
        /// <param name="item">The item to compare against the key.</param>
        /// <returns><see langword="true"/> if the item and key are associated, <see langword="false"/> otherwise.</returns>
        bool IsInstanceOf(NamespacedId id, Item item);

        /// <summary>
        /// Tries to create an instance of the specified item.
        /// </summary>
        /// <param name="id">The key for the item.</param>
        /// <param name="item">The created item, if successful, with a stack size of 1.</param>
        /// <returns><see langword="true"/> if successful, <see langword="false"/> otherwise.</returns>
        bool TryCreate(NamespacedId id, out Item item);

        /// <summary>
        /// Tries to get the sprite for a particular item.
        /// </summary>
        /// <param name="id">The item's key.</param>
        /// <param name="sprite">The sprite associated with the item.</param>
        /// <returns><see langword="true"/> if the sprite was retrieved, <see langword="false"/> otherwise.</returns>
        bool TryGetSprite(NamespacedId id, out ISprite sprite);
    }
}
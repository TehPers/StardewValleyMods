using StardewValley;

namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IItemProvider {
        /// <summary>Tries to create an item from the given key.</summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="item">If successful, the item that was created with a stack size of 1.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryCreateItem(ItemKey key, out Item item);

        /// <summary>Checks if an item is associated with a particular key.</summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="item">The item to compare against the key.</param>
        /// <returns>True if the item and key are associated, false otherwise.</returns>
        bool IsInstanceOf(ItemKey key, Item item);

        /// <summary>Invalidates any assets used by the items registered by this provider. This is called after each item is assigned an index, which occurs when a save is loaded or when connecting to a multiplayer game.</summary>
        void InvalidateAssets();
    }
}
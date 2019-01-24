namespace TehPers.CoreMod.Api.Items {
    public interface IItemDelegator {
        /// <summary>Tries to register a particular key. Once registered, an index will be assigned to the key whenever the item is available to be used.</summary>
        /// <param name="key">The key to register.</param>
        /// <returns>True if successful, false if the key is already registered.</returns>
        bool TryRegisterKey(ItemKey key);

        /// <summary>Tries to get the index associated with a particular key.</summary>
        /// <param name="key">The item key to get the index for.</param>
        /// <param name="index">The index assigned to the given key, if one is assigned.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetIndex(ItemKey key, out int index);
    }
}
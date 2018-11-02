namespace TehPers.CoreMod.Api.Items {
    public interface IItemApi {
        /// <summary>Registers a new type of object with a key.</summary>
        /// <param name="key">The unique key for this type of object. Key must be unique within your mod.</param>
        /// <param name="objectManager">The <see cref="IModObject"/> that will handle this type of object.</param>
        void Register(string key, IModObject objectManager);

        /// <summary>Gets the index associated with a particular key.</summary>
        /// <param name="key">The key of the type of object to get the index of.</param>
        /// <returns>The index associated with the given key, or <c>null</c> if not found.</returns>
        int? GetIndex(string key);

        /// <summary>Tries to get the index associated with a particular key.</summary>
        /// <param name="key">The key of the type of object to get the index of.</param>
        /// <param name="index">The index associated with the given key.</param>
        /// <returns>True if the key is registered and an index was found, false otherwise.</returns>
        bool TryGetIndex(string key, out int index);
    }
}

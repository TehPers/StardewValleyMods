using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Items {
    public interface IItemApi {
        /// <summary>Registers a new type of object with a key.</summary>
        /// <param name="localKey">The unique local key for this type of object. Key must be unique within your mod. The local key provided here will be used to create a global key.</param>
        /// <param name="objectManager">The <see cref="IModObject"/> that will handle this type of object.</param>
        /// <returns>The global key associated with the type of object registered.</returns>
        string Register(string localKey, IModObject objectManager);

        /// <summary>Gets the index associated with a particular key.</summary>
        /// <param name="key">The local or global key of the type of object to get the index of.</param>
        /// <returns>The index associated with the given key, or <c>null</c> if not found.</returns>
        /// <remarks>Checks if it matches a local key, then check for a global key.</remarks>
        int? GetIndex(string key);

        /// <summary>Tries to get the index associated with a particular key.</summary>
        /// <param name="key">The local or global key of the type of object to get the index of.</param>
        /// <param name="index">The index associated with the given key.</param>
        /// <returns>True if the key is registered and an index was found, false otherwise.</returns>
        /// <remarks>Checks if it matches a local key, then check for a global key.</remarks>
        bool TryGetIndex(string key, out int index);

        /// <summary>Gets all registered global keys.</summary>
        /// <returns>Every registered global key, even ones from other mods.</returns>
        IEnumerable<string> GetRegisteredKeys();
    }
}

using System.Collections.Generic;
using StardewValley;

namespace TehPers.CoreMod.Api.Items {
    public interface IItemApi {
        /// <summary>Registers a new type of object with a key.</summary>
        /// <param name="localKey">The unique local key for this type of object. Key must be unique within your mod. The local key provided here will be used to create a global key.</param>
        /// <param name="objectManager">The <see cref="IModObject"/> that will handle this type of object.</param>
        /// <returns>The global key associated with the type of object registered.</returns>
        string Register(string localKey, IModObject objectManager);

        /// <summary>Tries to get the information associated with a particular key.</summary>
        /// <param name="key">The local or global key of the type of object to get the information of.</param>
        /// <param name="objectInformation">The information associated with the given key.</param>
        /// <returns>True if the key is registered and information was found, false otherwise.</returns>
        /// <remarks>Checks if it matches a local key, then check for a global key.</remarks>
        bool TryGetInformation(string key, out IObjectInformation objectInformation);

        /// <summary>Tries to get the information associated with a particular index.</summary>
        /// <param name="index">The <see cref="Item.ParentSheetIndex"/> assigned to the type of object to get the information of.</param>
        /// <param name="objectInformation">The information associated with the given key.</param>
        /// <returns>True if the key is registered and information was found, false otherwise.</returns>
        bool TryGetInformation(int index, out IObjectInformation objectInformation);

        /// <summary>Gets all registered objects.</summary>
        /// <returns>Every registered object's information, even ones from other mods.</returns>
        IEnumerable<IObjectInformation> GetRegisteredObjects();
    }
}

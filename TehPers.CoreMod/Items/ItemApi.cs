using System;
using System.Collections.Generic;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Items {
    internal class ItemApi : IItemApi {
        private readonly ICoreApi _coreApi;

        public ItemApi(ICoreApi coreApi) {
            this._coreApi = coreApi;
        }

        public string Register(string localKey, IModObject objectManager) {
            // Convert the local key to a global key
            string globalKey = this.LocalToGlobal(localKey);

            // Try to register the key with the item delegator
            if (!ItemDelegator.Register(globalKey, objectManager)) {
                throw new InvalidOperationException($"An object with the key '{localKey}' has already been registered.");
            }

            // Return the global key
            return globalKey;
        }

        public bool TryGetInformation(string key, out IObjectInformation objectInformation) {
            return ItemDelegator.TryGetInformation(key, out objectInformation) || ItemDelegator.TryGetInformation(this.LocalToGlobal(key), out objectInformation);
        }

        public bool TryGetInformation(int index, out IObjectInformation objectInformation) {
            return ItemDelegator.TryGetInformation(index, out objectInformation);
        }

        public IEnumerable<IObjectInformation> GetRegisteredObjects() {
            return ItemDelegator.RegisteredObjects;
        }

        private string LocalToGlobal(string localKey) {
            return $"{this._coreApi.Owner.ModManifest.UniqueID}:{localKey}";
        }
    }
}
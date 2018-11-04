using System;
using System.Collections.Generic;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Internal.Items {
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

        public int? GetIndex(string key) {
            if (ItemDelegator.TryGetIndex(key, out int localIndex)) {
                return localIndex;
            }

            return ItemDelegator.TryGetIndex(this.LocalToGlobal(key), out int index) ? index : (int?) null;
        }

        public bool TryGetIndex(string key, out int index) {
            if (ItemDelegator.TryGetIndex(key, out int localIndex)) {
                index = localIndex;
                return true;
            }

            return ItemDelegator.TryGetIndex(this.LocalToGlobal(key), out index);
        }

        public IEnumerable<string> GetRegisteredKeys() {
            return ItemDelegator.RegisteredKeys;
        }

        private string LocalToGlobal(string localKey) {
            return $"{this._coreApi.Owner.ModManifest.UniqueID}:{localKey}";
        }
    }
}
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Internal.Items {
    internal class ItemApi : IItemApi {
        private readonly ICoreApi _coreApi;
        private readonly TextureAssetTracker _tracker;

        public ItemApi(ICoreApi coreApi, TextureAssetTracker tracker) {
            this._coreApi = coreApi;
            this._tracker = tracker;
        }

        public string Register(string localKey, IModObject objectManager) {
            // Convert the local key to a global key
            string globalKey = this.LocalToGlobal(localKey);

            // Try to register the key with the item delegator
            if (!ItemDelegator.Register(globalKey, objectManager, this._tracker)) {
                throw new InvalidOperationException($"An object with the key '{localKey}' has already been registered.");
            }

            // Return the global key
            return globalKey;
        }

        public bool TryGetIndex(string key, out int index) {
            return ItemDelegator.TryGetIndex(key, out index) || ItemDelegator.TryGetIndex(this.LocalToGlobal(key), out index);
        }

        public IEnumerable<string> GetRegisteredKeys() {
            return ItemDelegator.RegisteredKeys;
        }

        private string LocalToGlobal(string localKey) {
            return $"{this._coreApi.Owner.ModManifest.UniqueID}:{localKey}";
        }
    }
}
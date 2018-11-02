using System;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Internal.Items {
    internal class ItemApi : IItemApi {
        private readonly ICoreApi _coreApi;

        public ItemApi(ICoreApi coreApi) {
            this._coreApi = coreApi;
        }

        public void Register(string key, IModObject objectManager) {
            string actualKey = $"{this._coreApi.Owner.ModManifest.UniqueID}:{key}";
            if (!ItemDelegator.Register(actualKey, objectManager)) {
                throw new InvalidOperationException($"An object with the key '{key}' has already been registered.");
            }
        }
    }
}
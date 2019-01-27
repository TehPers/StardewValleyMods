using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Items;

namespace TehPers.CoreMod {
    public class CoreApiFactory : ICoreApiFactory {
        private readonly Dictionary<IMod, ICoreApi> _coreApis = new Dictionary<IMod, ICoreApi>();
        private readonly ItemDelegator _itemDelegator;

        internal CoreApiFactory(ItemDelegator itemDelegator) {
            this._itemDelegator = itemDelegator;
        }

        public ICoreApi GetApi(IMod mod) => this.GetApi(mod, null);
        public ICoreApi GetApi(IMod mod, Action<ICoreApiInitializer> initialize) {
            ICoreApi coreApi = this._coreApis.GetOrAdd(mod, () => new CoreApi(mod, this._itemDelegator));
            initialize?.Invoke(new CoreApiInitializer(coreApi));
            return coreApi;
        }
    }
}
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Drawing.Sprites;

namespace TehPers.CoreMod {
    internal class CoreApiFactory : ICoreApiFactory {
        private readonly Dictionary<IMod, ICoreApi> _coreApis = new Dictionary<IMod, ICoreApi>();
        public DynamicSpriteSheet CustomItemSpriteSheet { get; }

        public CoreApiFactory(DynamicSpriteSheet customItemSpriteSheet) {
            this.CustomItemSpriteSheet = customItemSpriteSheet;
        }

        public ICoreApi GetApi(IMod mod) => this.GetApi(mod, null);
        public ICoreApi GetApi(IMod mod, Action<ICoreApiInitializer> initialize) {
            ICoreApi coreApi = this._coreApis.GetOrAdd(mod, () => new CoreApi(mod, this.CustomItemSpriteSheet));
            initialize?.Invoke(new CoreApiInitializer(coreApi));
            return coreApi;
        }
    }
}
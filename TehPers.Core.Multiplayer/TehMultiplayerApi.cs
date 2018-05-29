using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using TehPers.Core.Multiplayer.Items;

namespace TehPers.Core.Multiplayer {
    public sealed class TehMultiplayerApi {
        private static readonly Dictionary<IMod, TehMultiplayerApi> _apis = new Dictionary<IMod, TehMultiplayerApi>();

        public IMod Mod { get; }

        private TehMultiplayerApi(IMod mod) {
            this.Mod = mod;
        }

        public static TehMultiplayerApi GetApi(IMod mod) {
            if (!TehMultiplayerApi._apis.TryGetValue(mod, out TehMultiplayerApi helper)) {
                helper = new TehMultiplayerApi(mod);
                TehMultiplayerApi._apis[mod] = helper;
            }

            return helper;
        }

        public void RegisterItem(int parentSheetIndex, ItemManager manager) {
            manager.Owner = this.Mod;
            ItemDelegator.SetManagerFor(parentSheetIndex, manager, this);
        }
    }
}

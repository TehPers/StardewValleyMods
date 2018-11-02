using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Static.Extensions;
using TehPers.CoreMod.Internal.Items;

namespace TehPers.CoreMod {
    public class ModCore : Mod {
        public const int StartingIndex = 1000000;

        private readonly Dictionary<IMod, ICoreApi> _coreApis = new Dictionary<IMod, ICoreApi>();

        public override void Entry(IModHelper helper) {

            // Add asset loader for custom items
            ItemsAssetEditor assetEditor = new ItemsAssetEditor();
            helper.Content.AssetEditors.Add(assetEditor);

            SaveEvents.AfterLoad += (sender, args) => {
                // Load all the key <-> index mapping for this save
                ItemDelegator.ReloadIndexes(this);

                // Save the current mapping in case any new ones were added
                ItemDelegator.SaveIndexes(this);

                // Reload object data
                ItemDelegator.Invalidate(this);
            };

            SaveEvents.AfterSave += (sender, args) => {
                // Save indexes for the save
                ItemDelegator.SaveIndexes(this);
            };

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        public override object GetApi() {
            // TODO: encapsulate in new type
            return new Func<IMod, ICoreApi>(mod => this._coreApis.GetOrAdd(mod, () => new CoreApi(this)));
        }
    }
}

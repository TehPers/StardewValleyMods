using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Items;
using TehPers.CoreMod.Items.Machines;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod {
    public class ModCore : Mod {
        private readonly Dictionary<IMod, ICoreApi> _coreApis = new Dictionary<IMod, ICoreApi>();

        public ModCore() {
            // Patch needs to happen in constructor otherwise it doesn't work with Better Artisan Good Icons for some reason.
            // If that mod patches SObject.drawWhenHeld before this mod patches SpriteBatch.Draw, then the items don't appear
            // in the farmer's hands when held.
            DrawingDelegator.PatchIfNeeded();

            // Also do other patches here because why not
            MachineDelegator.PatchIfNeeded(this);
        }

        public override void Entry(IModHelper helper) {
            // Add asset loader for custom items
            ItemsAssetEditor assetEditor = new ItemsAssetEditor();
            helper.Content.AssetEditors.Add(assetEditor);

            // Create texture asset tracker
            TextureAssetTracker tracker = new TextureAssetTracker(helper.Content);
            helper.Content.AssetEditors.Add(tracker);

            // Start overriding item draw calls
            ICoreApi coreApi = this.GetOrCreateCoreApi(this);
            ItemDelegator.OverrideDrawingIfNeeded(coreApi.Drawing, tracker);

            // Register events for the item delegator
            ItemDelegator.RegisterMultiplayerEvents(this);
            ItemDelegator.RegisterSaveEvents(this);

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        public override object GetApi() {
            // TODO: encapsulate in new type
            return new Func<IMod, ICoreApi>(this.GetOrCreateCoreApi);
        }

        private ICoreApi GetOrCreateCoreApi(IMod mod) {
            return this._coreApis.GetOrAdd(mod, () => new CoreApi(mod));
        }
    }
}

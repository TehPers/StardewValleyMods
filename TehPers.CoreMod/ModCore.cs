using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
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
            MachineDelegator.PatchIfNeeded();
        }

        public override void Entry(IModHelper helper) {

            // Add asset loader for custom items
            ItemsAssetEditor assetEditor = new ItemsAssetEditor();
            helper.Content.AssetEditors.Add(assetEditor);

            // Create texture asset tracker
            TextureAssetTracker tracker = new TextureAssetTracker(helper.Content);
            helper.Content.AssetEditors.Add(tracker);

            // Start overriding item draw calls
            ItemDelegator.OverrideDrawingIfNeeded(tracker);

            this.Helper.Events.GameLoop.SaveLoaded += (sender, args) => {
                // Load all the key <-> index mapping for this save
                ItemDelegator.ReloadIndexes(this); ;

                // Reload object data
                ItemDelegator.Invalidate(this);
            };

            this.Helper.Events.GameLoop.Saved += (sender, args) => {
                // Save indexes for the save
                ItemDelegator.SaveIndexes(this);
            };

            this.Helper.Events.GameLoop.ReturnedToTitle += (sender, args) => {
                // Clear all indexes when not in a save
                ItemDelegator.ClearIndexes(this);
            };

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        public override object GetApi() {
            // TODO: encapsulate in new type
            return new Func<IMod, ICoreApi>(mod => this._coreApis.GetOrAdd(mod, () => new CoreApi(mod)));
        }
    }
}

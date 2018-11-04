using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Static.Extensions;
using TehPers.CoreMod.Internal;
using TehPers.CoreMod.Internal.Drawing;
using TehPers.CoreMod.Internal.Items;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod {
    public class ModCore : Mod {
        private readonly Dictionary<IMod, ICoreApi> _coreApis = new Dictionary<IMod, ICoreApi>();
        private TextureAssetTracker _tracker;

        public ModCore() {
            // Patch needs to happen in constructor otherwise it doesn't work with Better Artisan Good Icons for some reason.
            // If that mod patches SObject.drawWhenHeld before this mod patches SpriteBatch.Draw, then the items don't appear
            // in the farmer's hands when held.
            DrawingDelegator.PatchIfNeeded();
        }

        public override void Entry(IModHelper helper) {

            // Add asset loader for custom items
            ItemsAssetEditor assetEditor = new ItemsAssetEditor();
            helper.Content.AssetEditors.Add(assetEditor);

            // Create texture asset tracker
            this._tracker = new TextureAssetTracker(helper.Content);
            helper.Content.AssetEditors.Add(this._tracker);

            SaveEvents.AfterLoad += (sender, args) => {
                // Load all the key <-> index mapping for this save
                ItemDelegator.ReloadIndexes(this); ;

                // Reload object data
                ItemDelegator.Invalidate(this);
            };

            SaveEvents.AfterSave += (sender, args) => {
                // Save indexes for the save
                ItemDelegator.SaveIndexes(this);
            };

            SaveEvents.AfterReturnToTitle += (sender, args) => {
                // Clear all indexes when not in a save
                ItemDelegator.ClearIndexes(this);
            };

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        private static void EmptyPatch() { }

        public override object GetApi() {
            // TODO: encapsulate in new type
            return new Func<IMod, ICoreApi>(mod => this._coreApis.GetOrAdd(mod, () => new CoreApi(mod, this._tracker)));
        }
    }
}

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

            GraphicsEvents.OnPreRenderHudEvent += (sender, args) => {
                if (Game1.player?.ActiveObject != null && Game1.currentLocation != null) {
                    //Game1.drawPlayerHeldObject(Game1.player);

                    Farmer f = Game1.player;
                    if (Game1.eventUp && (Game1.currentLocation.currentEvent == null || !Game1.currentLocation.currentEvent.showActiveObject) || (f.FarmerSprite.PauseForSingleAnimation || f.isRidingHorse() || f.bathingClothes.Value))
                        return;
                    float x = f.getLocalPosition(Game1.viewport).X + ((double) f.rotation < 0.0 ? -8f : ((double) f.rotation > 0.0 ? 8f : 0.0f)) + f.FarmerSprite.CurrentAnimationFrame.xOffset * 4;
                    float y = f.getLocalPosition(Game1.viewport).Y - 128f + f.FarmerSprite.CurrentAnimationFrame.positionOffset * 4 + FarmerRenderer.featureYOffsetPerFrame[f.FarmerSprite.CurrentAnimationFrame.frame] * 4;
                    if (f.ActiveObject.bigCraftable.Value)
                        y -= 64f;
                    if (f.isEating) {
                        x = f.getLocalPosition(Game1.viewport).X - 21f;
                        y = (float) (f.getLocalPosition(Game1.viewport).Y - 128.0 + 12.0);
                    }
                    if (f.isEating && (!f.isEating || f.Sprite.currentFrame > 218))
                        return;
                    f.ActiveObject.drawWhenHeld(Game1.spriteBatch, new Vector2((int) x, (int) y), f);
                }
            };

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        private static void EmptyPatch() { }

        public override object GetApi() {
            // TODO: encapsulate in new type
            return new Func<IMod, ICoreApi>(mod => this._coreApis.GetOrAdd(mod, () => new CoreApi(mod)));
        }
    }
}

using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.ContentPacks;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Integration;
using TehPers.CoreMod.Items;
using TehPers.CoreMod.Items.Machines;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod {
    public class ModCore : Mod {
        private CoreApiFactory _coreApiFactory;
        private readonly ItemDelegator2 _itemDelegator;

        public ModCore() {
            // Patch needs to happen in constructor otherwise it doesn't work with Better Artisan Good Icons for some reason.
            // If that mod patches SObject.drawWhenHeld before this mod patches SpriteBatch.Draw, then the items don't appear
            // in the farmer's hands when held.
            DrawingDelegator.PatchIfNeeded();

            // Also do other patches here because why not
            MachineDelegator.PatchIfNeeded();

            // Create the item delegator
            this._itemDelegator = new ItemDelegator2(this);
        }

        public override void Entry(IModHelper helper) {
            // Create the custom item sprite sheet
            this._coreApiFactory = new CoreApiFactory(this._itemDelegator);

            // Create texture asset tracker
            TextureAssetTracker tracker = new TextureAssetTracker(helper.Content);
            helper.Content.AssetEditors.Add(tracker);

            // Start overriding item draw calls
            ICoreApi coreApi = this._coreApiFactory.GetApi(this);

            // Register events for the item delegator
            this._itemDelegator.Initialize();

            // Register events for the machine delegator
            // TODO: MachineDelegator.RegisterEvents(this);

            // Load content packs after the game is launched
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => this.Helper.Events.GameLoop.UpdateTicking += this.UpdateTicking_LoadContentPacks;
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => this.LoadIntegrations(coreApi);

            // Toggle sprite sheet command
            bool spriteSheetVisible = false;
            this.Helper.ConsoleCommands.Add("core_togglespritesheet", "Toggles drawing the custom item sprite sheet in the top left corner of the screen.", (s, strings) => {
                spriteSheetVisible = !spriteSheetVisible;
                if (spriteSheetVisible) {
                    this.Helper.Events.Display.RenderingHud += this.OnRenderingHud_DisplaySpriteSheet;
                } else {
                    this.Helper.Events.Display.RenderingHud -= this.OnRenderingHud_DisplaySpriteSheet;
                }
            });

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        private void LoadIntegrations(ICoreApi coreApi) {
            if (this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets") is IJsonAssetsApi jsonAssetsApi) {
                JsonAssetsItemProvider itemProvider = new JsonAssetsItemProvider(coreApi, jsonAssetsApi);
                // TODO: Add JA item provider
            }
        }

        private void UpdateTicking_LoadContentPacks(object sender, UpdateTickingEventArgs e) {
            // Remove this event handler so it only fires once
            this.Helper.Events.GameLoop.UpdateTicking -= this.UpdateTicking_LoadContentPacks;

            // Load content packs
            ContentPackLoader contentPackLoader = new ContentPackLoader(this._coreApiFactory.GetApi(this));
            contentPackLoader.LoadContentPacks();
        }

        private void OnRenderingHud_DisplaySpriteSheet(object sender, RenderingHudEventArgs args) {
            args.SpriteBatch.Draw(this._itemDelegator.CustomItemSpriteSheet.TrackedTexture.CurrentTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
        }

        public override object GetApi() {
            return this._coreApiFactory;
        }
    }
}

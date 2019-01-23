using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.ContentPacks;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Items;
using TehPers.CoreMod.Items.Machines;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod {
    public class ModCore : Mod {
        private CoreApiFactory _coreApiFactory;

        public ModCore() {
            // Patch needs to happen in constructor otherwise it doesn't work with Better Artisan Good Icons for some reason.
            // If that mod patches SObject.drawWhenHeld before this mod patches SpriteBatch.Draw, then the items don't appear
            // in the farmer's hands when held.
            DrawingDelegator.PatchIfNeeded();

            // Also do other patches here because why not
            MachineDelegator.PatchIfNeeded();
        }

        public override void Entry(IModHelper helper) {
            // Create the custom item sprite sheet
            this._coreApiFactory = new CoreApiFactory(new DynamicSpriteSheet(this));

            // Add asset loader for custom items
            ItemsAssetEditor assetEditor = new ItemsAssetEditor();
            helper.Content.AssetEditors.Add(assetEditor);

            // Create texture asset tracker
            TextureAssetTracker tracker = new TextureAssetTracker(helper.Content);
            helper.Content.AssetEditors.Add(tracker);

            // Start overriding item draw calls
            ICoreApi coreApi = this._coreApiFactory.GetApi(this);
            ItemDelegator.OverrideDrawingIfNeeded(coreApi.Drawing, tracker);

            // Register events for the item delegator
            ItemDelegator.RegisterMultiplayerEvents(this);
            ItemDelegator.RegisterSaveEvents(this);

            // Register events for the machine delegator
            MachineDelegator.RegisterEvents(this);

            // Load content packs after the game is launched
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => this.Helper.Events.GameLoop.UpdateTicking += this.UpdateTicking_LoadContentPacks;

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

        private void UpdateTicking_LoadContentPacks(object sender, UpdateTickingEventArgs e) {
            // Remove this event handler so it only fires once
            this.Helper.Events.GameLoop.UpdateTicking -= this.UpdateTicking_LoadContentPacks;

            // Load content packs
            ContentPackLoader contentPackLoader = new ContentPackLoader(this._coreApiFactory.GetApi(this));
            contentPackLoader.LoadContentPacks();
        }

        private void OnRenderingHud_DisplaySpriteSheet(object sender, RenderingHudEventArgs args) {
            args.SpriteBatch.Draw(this._coreApiFactory.CustomItemSpriteSheet.TrackedTexture.CurrentTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
        }

        public override object GetApi() {
            return this._coreApiFactory;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Structs;
using TehPers.CoreMod.Drawing.Sprites;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingApi : IDrawingApi {
        private static readonly Lazy<Texture2D> _whitePixel = new Lazy<Texture2D>(() => {
            Texture2D whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            return whitePixel;
        });

        private readonly IApiHelper _coreApiHelper;
        private readonly Dictionary<AssetLocation, TrackedTexture> _textureHelpers = new Dictionary<AssetLocation, TrackedTexture>();
        private readonly TextureTracker _textureTracker;

        public Texture2D WhitePixel => DrawingApi._whitePixel.Value;

        public DrawingApi(IApiHelper coreApiHelper) {
            this._coreApiHelper = coreApiHelper;
            this._textureTracker = new TextureTracker(this._coreApiHelper);
            coreApiHelper.Owner.Helper.Content.AssetEditors.Add(this._textureTracker);

            this.ObjectSpriteSheet = new SpriteSheet(this.GetTrackedTexture(new AssetLocation("Maps/springobjects", ContentSource.GameContent)), 16, 16);
            this.CraftableSpriteSheet = new SpriteSheet(this.GetTrackedTexture(new AssetLocation("TileSheets/Craftables", ContentSource.GameContent)), 16, 32);
        }

        public ITrackedTexture GetTrackedTexture(AssetLocation asset) {
            // Check if this asset is already being tracked
            if (this._textureHelpers.TryGetValue(asset, out TrackedTexture trackedTexture)) {
                return trackedTexture;
            }

            try {
                // Load the texture
                Texture2D texture = asset.Load<Texture2D>(this._coreApiHelper.Owner.Helper.Content);

                // Create a new helper
                trackedTexture = new TrackedTexture(texture);
                this._textureHelpers.Add(asset, trackedTexture);

                // Start tracking changes to the asset, and register the helper with the DrawingDelegator
                this._textureTracker.AddHelper(asset, trackedTexture);

                // Return the tracked texture
                return trackedTexture;
            } catch (ContentLoadException ex) {
                // Failed to load the texture, so throw a meaningful error
                throw new InvalidOperationException($"The requested texture failed to load: {ex.Message}", ex);
            }
        }

        public ISpriteSheet CreateSimpleSpriteSheet(ITrackedTexture trackedTexture, int spriteWidth, int spriteHeight) {
            string preferredProperty = trackedTexture.CurrentTexture.Match<Texture2D, string>()
                .When(this.ObjectSpriteSheet.TrackedTexture.CurrentTexture, nameof(DrawingApi.ObjectSpriteSheet))
                .When(this.CraftableSpriteSheet.TrackedTexture.CurrentTexture, nameof(DrawingApi.CraftableSpriteSheet))
                .Else((string) null);

            if (preferredProperty != null) {
                this._coreApiHelper.Owner.Monitor.Log($"A new sprite sheet was created for a texture which already has a sprite sheet provided. {nameof(IDrawingInfo)}.{preferredProperty} should be used instead.", LogLevel.Warn);
            }

            return new SpriteSheet(trackedTexture, spriteWidth, spriteHeight);
        }

        /// <inheritdoc />
        public ISpriteSheet ObjectSpriteSheet { get; }

        /// <inheritdoc />
        public ISpriteSheet CraftableSpriteSheet { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingApi : IDrawingApi {
        private static readonly Lazy<Texture2D> _whitePixel = new Lazy<Texture2D>(() => {
            Texture2D whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            return whitePixel;
        });

        private readonly IApiHelper _coreApiHelper;
        private readonly Dictionary<GameAssetLocation, TextureDrawingHelper> _textureHelpers = new Dictionary<GameAssetLocation, TextureDrawingHelper>();
        private readonly TextureTracker _textureTracker;

        public Texture2D WhitePixel => DrawingApi._whitePixel.Value;

        public DrawingApi(IApiHelper coreApiHelper) {
            this._coreApiHelper = coreApiHelper;
            this._textureTracker = new TextureTracker(this);
            coreApiHelper.Owner.Helper.Content.AssetEditors.Add(this._textureTracker);
        }

        public ITextureDrawingHelper GetTextureHelper(GameAssetLocation gameAsset) {
            if (this._textureHelpers.TryGetValue(gameAsset, out TextureDrawingHelper helper)) {
                return helper;
            }

            // Create a new helper
            helper = new TextureDrawingHelper(this);
            this._textureHelpers.Add(gameAsset, helper);

            // Start tracking changes to the asset, and register the helper with the DrawingDelegator
            this._textureTracker.AddHelper(gameAsset, helper);

            return helper;
        }
    }
}
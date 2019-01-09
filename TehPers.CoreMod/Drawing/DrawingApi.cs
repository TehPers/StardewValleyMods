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

        public Texture2D WhitePixel => DrawingApi._whitePixel.Value;

        public DrawingApi(IApiHelper coreApiHelper) {
            this._coreApiHelper = coreApiHelper;
        }

        public ITextureDrawingHelper GetTextureHelper(GameAssetLocation gameAsset) {
            if (this._textureHelpers.TryGetValue(gameAsset, out TextureDrawingHelper helper)) {
                return helper;
            }

            // Create a new helper
            helper = new TextureDrawingHelper(this);
            this._textureHelpers.Add(gameAsset, helper);

            // Bind it to the texture
            RegisterTextureHelper();

            // TODO: Keep updating it whenever the asset is invalidated

            return helper;

            void RegisterTextureHelper() {
                Texture2D texture = gameAsset.Source.Match<ContentSource, Texture2D>()
                    .When(ContentSource.GameContent, () => Game1.content.Load<Texture2D>(gameAsset.Path))
                    .When(ContentSource.ModFolder, () => this._coreApiHelper.Owner.Helper.Content.Load<Texture2D>(gameAsset.Path))
                    .ElseThrow();

                DrawingDelegator.AddTextureHelper(texture, helper);
            }
        }
    }
}
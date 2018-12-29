using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingApi : IDrawingApi {
        private static readonly Lazy<Texture2D> _whitePixel = new Lazy<Texture2D>(() => {
            Texture2D whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            return whitePixel;
        });

        private readonly ICoreApi _coreApi;

        public Texture2D WhitePixel => DrawingApi._whitePixel.Value;

        public DrawingApi(ICoreApi coreApi) {
            this._coreApi = coreApi;
        }

        public void AddOverride(Action<IDrawingInfo> overrider) {
            DrawingDelegator.AddOverride(overrider);
        }

        public bool RemoveOverride(Action<IDrawingInfo> overrider) {
            return DrawingDelegator.RemoveOverride(overrider);
        }
    }
}
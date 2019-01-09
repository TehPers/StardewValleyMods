using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Drawing {
    internal class ReadonlyDrawingInfo : IReadonlyDrawingInfo {
        public Vector2 Scale { get; }
        public Rectangle? SourceRectangle { get; }
        public Texture2D Texture { get; }
        public Color Tint { get; }

        public ReadonlyDrawingInfo(Vector2 scale, Rectangle? sourceRectangle, Texture2D texture, Color tint) {
            this.Scale = scale;
            this.SourceRectangle = sourceRectangle;
            this.Texture = texture;
            this.Tint = tint;
        }

        public ReadonlyDrawingInfo(IReadonlyDrawingInfo info) {
            this.Scale = info.Scale;
            this.SourceRectangle = info.SourceRectangle;
            this.Texture = info.Texture;
            this.Tint = info.Tint;
        }
    }
}
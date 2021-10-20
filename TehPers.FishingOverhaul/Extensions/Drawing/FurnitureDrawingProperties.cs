using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    public record FurnitureDrawingProperties
        (Vector2 SourceSize, float ScaleSize) : IDrawingProperties
    {
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => this.SourceSize / 2f;
        public float RealScale(float scaleSize) => this.ScaleSize * scaleSize;
    }
}
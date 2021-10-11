using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    public record ClothingDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 16f);
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => new(8f, 8f);
        public float RealScale(float scaleSize) => 4f * scaleSize;
    }
}
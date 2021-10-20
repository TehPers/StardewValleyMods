using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal record BootsDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 16f);
        public Vector2 Offset(float scaleSize) => new(32f * scaleSize, 32f * scaleSize);
        public Vector2 Origin(float scaleSize) => new(8f * scaleSize, 8f * scaleSize);
        public float RealScale(float scaleSize) => 4f * scaleSize;
    }
}
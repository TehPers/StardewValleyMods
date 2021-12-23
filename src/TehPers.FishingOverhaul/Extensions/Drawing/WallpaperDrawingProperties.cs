using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal record WallpaperDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 28f);
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => new(8f, 14f);
        public float RealScale(float scaleSize) => 2f * scaleSize;
    }
}
using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    public record ObjectDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 16f);
        public Vector2 Offset(float scaleSize) => new(32f * scaleSize, 32f * scaleSize);
        public Vector2 Origin(float scaleSize) => new(8f * scaleSize, 8f * scaleSize);
        public float RealScale(float scaleSize) => 4f * scaleSize;
    }

    public record FurnitureDrawingProperties
        (Vector2 SourceSize, float ScaleSize) : IDrawingProperties
    {
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => this.SourceSize / 2f;
        public float RealScale(float scaleSize) => this.ScaleSize * scaleSize;
    }

    public record HatDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(20f, 20f);
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => new(10f, 10f);
        public float RealScale(float scaleSize) => 4f * scaleSize;
    }

    public record RingDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 16f);
        public Vector2 Offset(float scaleSize) => new(32f * scaleSize, 32f * scaleSize);
        public Vector2 Origin(float scaleSize) => new(8f * scaleSize, 8f * scaleSize);
        public float RealScale(float scaleSize) => 4f * scaleSize;
    }

    public record ToolDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 16f);
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => new(8f, 8f);
        public float RealScale(float scaleSize) => 4f * scaleSize;
    }
}
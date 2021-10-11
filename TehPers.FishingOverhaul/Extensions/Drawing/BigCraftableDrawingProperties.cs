using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    public record BigCraftableDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 32f);
        public Vector2 Offset(float scaleSize) => new(32f, 32f);
        public Vector2 Origin(float scaleSize) => new(8f, 16f);
        public float RealScale(float scaleSize) => 4f * (scaleSize < 0.2f ? scaleSize : scaleSize / 2f);
    }
}
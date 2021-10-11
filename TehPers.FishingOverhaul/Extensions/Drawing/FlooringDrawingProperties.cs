using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    public record FlooringDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(28f, 26f);
        public Vector2 Offset(float scaleSize) => new(32f, 30f);
        public Vector2 Origin(float scaleSize) => new(14f, 13f);
        public float RealScale(float scaleSize) => 2f * scaleSize;
    }
}
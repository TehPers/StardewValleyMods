using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal record ClothingDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(16f, 16f);

        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 32f);
        }

        public Vector2 Origin(float scaleSize)
        {
            return new(8f, 8f);
        }

        public float RealScale(float scaleSize)
        {
            return 4f * scaleSize;
        }
    }
}
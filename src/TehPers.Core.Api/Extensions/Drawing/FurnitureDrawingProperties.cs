using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    public record FurnitureDrawingProperties
        (Vector2 SourceSize, float ScaleSize) : IDrawingProperties
    {
        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 32f);
        }

        public Vector2 Origin(float scaleSize)
        {
            return this.SourceSize / 2f;
        }

        public float RealScale(float scaleSize)
        {
            return this.ScaleSize * scaleSize;
        }
    }
}
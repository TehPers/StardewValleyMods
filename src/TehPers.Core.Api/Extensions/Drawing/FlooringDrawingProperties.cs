using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    public record FlooringDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(28f, 26f);

        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 30f);
        }

        public Vector2 Origin(float scaleSize)
        {
            return new(14f, 13f);
        }

        public float RealScale(float scaleSize)
        {
            return 2f * scaleSize;
        }
    }
}
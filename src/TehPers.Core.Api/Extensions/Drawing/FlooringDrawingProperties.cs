using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    /// <summary>
    /// Properties about how flooring is drawn in menus.
    /// </summary>
    public record FlooringDrawingProperties : IDrawingProperties
    {
        /// <inheritdoc />
        public Vector2 SourceSize => new(28f, 26f);

        /// <inheritdoc />
        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 30f);
        }

        /// <inheritdoc />
        public Vector2 Origin(float scaleSize)
        {
            return new(14f, 13f);
        }

        /// <inheritdoc />
        public float RealScale(float scaleSize)
        {
            return 2f * scaleSize;
        }
    }
}

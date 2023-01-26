using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    /// <summary>
    /// Properties about how hats are drawn in menus.
    /// </summary>
    public record HatDrawingProperties : IDrawingProperties
    {
        /// <inheritdoc/>
        public Vector2 SourceSize => new(20f, 20f);

        /// <inheritdoc/>
        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 32f);
        }

        /// <inheritdoc/>
        public Vector2 Origin(float scaleSize)
        {
            return new(10f, 10f);
        }

        /// <inheritdoc/>
        public float RealScale(float scaleSize)
        {
            return 4f * scaleSize;
        }
    }
}
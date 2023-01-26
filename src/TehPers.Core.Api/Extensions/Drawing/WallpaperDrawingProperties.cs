using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    /// <summary>
    /// Properties about how wallpaper is drawn in menus.
    /// </summary>
    public record WallpaperDrawingProperties : IDrawingProperties
    {
        /// <inheritdoc/>
        public Vector2 SourceSize => new(16f, 28f);

        /// <inheritdoc/>
        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 32f);
        }

        /// <inheritdoc/>
        public Vector2 Origin(float scaleSize)
        {
            return new(8f, 14f);
        }

        /// <inheritdoc/>
        public float RealScale(float scaleSize)
        {
            return 2f * scaleSize;
        }
    }
}
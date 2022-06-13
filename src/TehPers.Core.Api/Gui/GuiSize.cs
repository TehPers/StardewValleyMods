using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A size for use in a GUI.
    /// </summary>
    /// <param name="Width">The width of the component.</param>
    /// <param name="Height">The height of the component.</param>
    public record GuiSize(float Width, float Height)
    {
        /// <summary>
        /// A size of zero in both axes.
        /// </summary>
        public static GuiSize Zero => new(0, 0);

        /// <summary>
        /// A size of one in both axes.
        /// </summary>
        public static GuiSize One => new(1, 1);

        /// <summary>
        /// Converts a <see cref="Vector2"/> to a <see cref="GuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public GuiSize(Vector2 size)
            : this(size.X, size.Y)
        {
        }

        /// <summary>
        /// Converts a <see cref="Point"/> to a <see cref="GuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public GuiSize(Point size)
            : this(size.X, size.Y)
        {
        }
    }
}

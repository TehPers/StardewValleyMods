using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A partially defined size for use in a GUI.
    /// </summary>
    /// <param name="Width">The width of the component, if any.</param>
    /// <param name="Height">The height of the component, if any.</param>
    public record PartialGuiSize(float? Width, float? Height)
    {
        /// <summary>
        /// An empty partial size. The size in each axis is <see langword="null"/>.
        /// </summary>
        public static PartialGuiSize Empty => new(null, null);

        /// <summary>
        /// A size of zero in both axes.
        /// </summary>
        public static PartialGuiSize Zero => new(0, 0);

        /// <summary>
        /// A size of one in both axes.
        /// </summary>
        public static PartialGuiSize One => new(1, 1);

        /// <summary>
        /// Converts a <see cref="GuiSize"/> to a <see cref="PartialGuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public PartialGuiSize(GuiSize size)
            : this(size.Width, size.Height)
        {
        }

        /// <summary>
        /// Converts a <see cref="Vector2"/> to a <see cref="PartialGuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public PartialGuiSize(Vector2 size)
            : this(size.X, size.Y)
        {
        }

        /// <summary>
        /// Converts a <see cref="Point"/> to a <see cref="PartialGuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public PartialGuiSize(Point size)
            : this(size.X, size.Y)
        {
        }
    }
}

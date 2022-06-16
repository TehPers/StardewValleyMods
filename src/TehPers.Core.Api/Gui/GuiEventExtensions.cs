using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Useful extension methods for <see cref="GuiEvent"/>.
    /// </summary>
    public static class GuiEventExtensions
    {
        /// <summary>
        /// Draws if the event is <see cref="GuiEvent.Draw"/>.
        /// </summary>
        /// <param name="e">The GUI event.</param>
        /// <param name="draw">A callback which draws.</param>
        public static void Draw(this GuiEvent e, Action<SpriteBatch> draw)
        {
            if (e is GuiEvent.Draw(var batch))
            {
                draw(batch);
            }
        }

        /// <summary>
        /// Checks whether the given bounds were clicked.
        /// </summary>
        /// <param name="e">The GUI event.</param>
        /// <param name="bounds">The bounds to check for clicks within.</param>
        /// <param name="clickType">The type of click to check for.</param>
        /// <returns>Whether the given bounds were clicked.</returns>
        public static bool Clicked(this GuiEvent e, Rectangle bounds, ClickType clickType)
        {
            return e.ClickType(bounds) == clickType;
        }

        /// <summary>
        /// Gets the type of click that was made within certain bounds.
        /// </summary>
        /// <param name="e">The GUI event.</param>
        /// <param name="bounds">The bounds to check for clicks within.</param>
        /// <returns>The type of click that was made, or <see langword="null"/> if no click was made within the bounds.</returns>
        public static ClickType? ClickType(this GuiEvent e, Rectangle bounds)
        {
            return e is GuiEvent.ReceiveClick(var position, var type) && bounds.Contains(position)
                ? type
                : null;
        }
    }
}

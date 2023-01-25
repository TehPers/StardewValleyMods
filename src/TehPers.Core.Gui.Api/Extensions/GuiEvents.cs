using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Api.Extensions;

/// <summary>
/// Convenient extension methods for <see cref="GuiEvent"/>.
/// </summary>
public static class GuiEvents
{
    /// <summary>
    /// Draws if the event is <see cref="GuiEvent.Draw"/>.
    /// </summary>
    /// <param name="e">The GUI event.</param>
    /// <param name="draw">A callback which draws.</param>
    public static void Draw(this IGuiEvent e, Action<SpriteBatch> draw)
    {
        if (e.IsDraw(out var batch))
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
    public static bool Clicked(this IGuiEvent e, Rectangle bounds, ClickType clickType)
    {
        return e.ClickType(bounds) == clickType;
    }

    /// <summary>
    /// Gets the type of click that was made within certain bounds.
    /// </summary>
    /// <param name="e">The GUI event.</param>
    /// <param name="bounds">The bounds to check for clicks within.</param>
    /// <returns>The type of click that was made, or <see langword="null"/> if no click was made within the bounds.</returns>
    public static ClickType? ClickType(this IGuiEvent e, Rectangle bounds)
    {
        return e.IsReceiveClick(out var position, out var type) && bounds.Contains(position)
            ? type
            : null;
    }
}

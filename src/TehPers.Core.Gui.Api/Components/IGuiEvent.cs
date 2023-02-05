using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// An event that can update the state of the GUI.
/// </summary>
public interface IGuiEvent
{
    /// <summary>
    /// Whether this event has already been handled.
    /// </summary>
    bool IsHandled { get; }

    /// <summary>
    /// Marks this event as handled.
    /// </summary>
    void Handle();

    /// <summary>
    /// Checks whether this was caused by a regular update tick.
    /// </summary>
    /// <param name="time">The current game time.</param>
    bool IsUpdateTick([MaybeNullWhen(false)] out GameTime time)
    {
        time = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by a position being hovered over.
    /// </summary>
    /// <param name="position">The position being hovered over.</param>
    bool IsHover(out Point position)
    {
        position = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by a mouse click.
    /// </summary>
    /// <param name="position">The cursor position when receiving the click.</param>
    /// <param name="clickType">The type of click received.</param>
    bool IsReceiveClick(out Point position, out ClickType clickType)
    {
        position = default;
        clickType = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by the mouse being scrolled.
    /// </summary>
    /// <param name="direction">The direction and amount the mouse wheel was scrolled.</param>
    bool IsScroll(out int direction)
    {
        direction = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by text being input by the user.
    /// </summary>
    /// <param name="text">The text that was input.</param>
    bool IsTextInput([MaybeNullWhen(false)] out string text)
    {
        text = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by a keyboard key press.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    bool IsKeyboardInput(out Keys key)
    {
        key = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by a game pad button press.
    /// </summary>
    /// <param name="button">The button that was pressed.</param>
    bool IsGamePadInput(out Buttons button)
    {
        button = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by the component being drawn.
    /// </summary>
    /// <param name="batch">The sprite batch to draw with.</param>
    bool IsDraw([MaybeNullWhen(false)] out SpriteBatch batch)
    {
        batch = default;
        return false;
    }

    /// <summary>
    /// Checks whether this was caused by the focus of this component being changed.
    /// </summary>
    /// <param name="focused">Whether this component is now focused.</param>
    /// <returns>Whether this event was caused by the focus being changed.</returns>
    bool IsFocusChanged(out bool focused)
    {

    }

    /// <summary>
    /// Checks whether this was caused by a custom event.
    /// </summary>
    /// <param name="data">The event data.</param>
    /// <returns>Whether this event was caused by a custom event.</returns>
    bool IsOther([MaybeNullWhen(false)] out object data)
    {
        data = default;
        return false;
    }
}

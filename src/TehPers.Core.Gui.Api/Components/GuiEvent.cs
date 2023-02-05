using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TehPers.Core.Gui.Api.Components;

/// <inheritdoc />
public abstract record GuiEvent : IGuiEvent
{
    /// <inheritdoc />
    public bool IsHandled => this.GetIsHandled?.Invoke() ?? false;

    /// <summary>
    /// A callback to mark this event as handled.
    /// </summary>
    internal Action? SetHandled { get; init; }

    /// <summary>
    /// A callback to check whether this event has been handled.
    /// </summary>
    internal Func<bool>? GetIsHandled { get; init; }

    private GuiEvent() { }

    /// <inheritdoc />
    public void Handle()
    {
        this.SetHandled?.Invoke();
    }

    /// <summary>
    /// A regular update tick.
    /// </summary>
    /// <param name="Time">The current game time.</param>
    public sealed record UpdateTick(GameTime Time) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsUpdateTick(out GameTime time)
        {
            time = this.Time;
            return true;
        }
    }

    /// <summary>
    /// A position was hovered over by the cursor.
    /// </summary>
    /// <param name="Position">The position being hovered over.</param>
    public sealed record Hover(Point Position) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsHover(out Point position)
        {
            position = this.Position;
            return true;
        }
    }

    /// <summary>
    /// A mouse click was received.
    /// </summary>
    /// <param name="Position">The cursor position when receiving the click.</param>
    /// <param name="ClickType">The type of click received.</param>
    public sealed record ReceiveClick(Point Position, ClickType ClickType) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsReceiveClick(out Point position, out ClickType clickType)
        {
            position = this.Position;
            clickType = this.ClickType;
            return true;
        }
    }

    /// <summary>
    /// The mouse was scrolled.
    /// </summary>
    /// <param name="Direction">The direction and amount the mouse wheel was scrolled.</param>
    public sealed record Scroll(int Direction) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsScroll(out int direction)
        {
            direction = this.Direction;
            return true;
        }
    }

    /// <summary>
    /// Text was input by the user.
    /// </summary>
    /// <param name="Text">The text that was input.</param>
    public sealed record TextInput(string Text) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsTextInput(out string text)
        {
            text = this.Text;
            return true;
        }
    }

    /// <summary>
    /// A keyboard key press was received.
    /// </summary>
    /// <param name="Key">The key that was pressed.</param>
    public sealed record KeyboardInput(Keys Key) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsKeyboardInput(out Keys key)
        {
            key = this.Key;
            return true;
        }
    }

    /// <summary>
    /// A game pad button press was received.
    /// </summary>
    /// <param name="Button">The button that was pressed.</param>
    public sealed record GamePadInput(Buttons Button) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsGamePadInput(out Buttons button)
        {
            button = this.Button;
            return true;
        }
    }

    /// <summary>
    /// The component is being drawn.
    /// </summary>
    /// <param name="Batch">The sprite batch to draw with.</param>
    public sealed record Draw(SpriteBatch Batch) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsDraw(out SpriteBatch batch)
        {
            batch = this.Batch;
            return true;
        }
    }

    /// <summary>
    /// A custom event.
    /// </summary>
    /// <param name="Data">The event data.</param>
    public sealed record Other(object Data) : GuiEvent, IGuiEvent
    {
        /// <inheritdoc />
        public bool IsOther(out object data)
        {
            data = this.Data;
            return true;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// An event that can update the state of the GUI.
    /// </summary>
    public abstract record GuiEvent
    {
        private GuiEvent() { }

        /// <summary>
        /// A regular update tick.
        /// </summary>
        /// <param name="Time">The current game time.</param>
        public sealed record UpdateTick(GameTime Time) : GuiEvent;

        /// <summary>
        /// A mouse click was received.
        /// </summary>
        /// <param name="Position">The cursor position when receiving the click.</param>
        /// <param name="ClickType">The type of click received.</param>
        public sealed record ReceiveClick(Point Position, ClickType ClickType) : GuiEvent;

        /// <summary>
        /// The mouse was scrolled.
        /// </summary>
        /// <param name="Direction">The direction and amount the mouse wheel was scrolled.</param>
        public sealed record Scroll(int Direction) : GuiEvent;

        /// <summary>
        /// Text was input by the user.
        /// </summary>
        /// <param name="Text">The text that was input.</param>
        public sealed record TextInput(string Text) : GuiEvent;

        /// <summary>
        /// A keyboard key press was received.
        /// </summary>
        /// <param name="Key">The key that was pressed.</param>
        public sealed record KeyboardInput(Keys Key) : GuiEvent;

        /// <summary>
        /// A game pad button press was received.
        /// </summary>
        /// <param name="Button">The button that was pressed.</param>
        public sealed record GamePadInput(Buttons Button) : GuiEvent;

        /// <summary>
        /// The component is being drawn.
        /// </summary>
        /// <param name="Batch">The sprite batch to draw with.</param>
        public sealed record Draw(SpriteBatch Batch) : GuiEvent;

        /// <summary>
        /// A custom event.
        /// </summary>
        /// <param name="Data">The event data.</param>
        public sealed record Other(object Data) : GuiEvent;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A text input component.
    /// </summary>
    public class TextBox : IGuiComponent, IKeyboardSubscriber
    {
        private readonly State state;

        /// <summary>
        /// The font of the text in the input.
        /// </summary>
        public SpriteFont Font { get; init; } = Game1.smallFont;

        /// <summary>
        /// Creates a new <see cref="TextBox"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TextBox(State state)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            // Focus handling
            if (e.Clicked(bounds, ClickType.Left))
            {
                this.state.Focused = true;
            }
            else if (e is GuiEvent.ReceiveClick)
            {
                this.state.Focused = false;
            }

            // Text input
            if (e is GuiEvent.TextInput(var key, var character))
            {
            }
        }


        /// <inheritdoc />
        public void RecieveTextInput(char inputChar)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void RecieveTextInput(string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void RecieveCommandInput(char command)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void RecieveSpecialInput(Keys key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Selected { get; set; }

        /// <summary>
        /// The state for a <see cref="TextBox"/>.
        /// </summary>
        public class State
        {
            public string Text { get; set; } = string.Empty;

            public bool Focused { get; internal set; } = false;
        }
    }
}

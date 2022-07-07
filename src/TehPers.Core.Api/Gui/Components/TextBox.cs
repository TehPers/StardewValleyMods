using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Gui.States;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// A standard text box.
    /// </summary>
    internal record TextBox : ComponentWrapper
    {
        private readonly IGuiComponent textInput;

        public override IGuiComponent Inner => this.CreateInner();

        /// <summary>
        /// Creates a new text box.
        /// </summary>
        /// <param name="state">The state of the text input.</param>
        /// <param name="inputHelper">The input helper.</param>
        public TextBox(TextInputState state, IInputHelper inputHelper)
            : this(
                new TextInput(state, inputHelper)
                {
                    HighlightedTextBackgroundColor = new(Color.DeepSkyBlue, 0.5f),
                    CursorColor = new(Color.Black, 0.75f),
                }
            )
        {
        }

        /// <summary>
        /// Creates a new text box.
        /// </summary>
        /// <param name="textInput">The inner text input.</param>
        public TextBox(IGuiComponent textInput)
        {
            this.textInput = textInput;
        }

        private IGuiComponent CreateInner()
        {
            var background = Game1.content.Load<Texture2D>(@"LooseSprites\textBox");
            return this.textInput.WithPadding(16, 6, 6, 8)
                .WithBackground(new TextureBox(background));
        }
    }
}

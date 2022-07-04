using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A standard text box.
    /// </summary>
    internal record TextBox : WrapperComponent
    {
        private readonly TextInputState state;
        private readonly IInputHelper inputHelper;

        protected override IGuiComponent Inner => this.CreateInner(this.state, this.inputHelper);

        /// <summary>
        /// Creates a new text box.
        /// </summary>
        /// <param name="state">The state of the text input.</param>
        /// <param name="inputHelper">The input helper.</param>
        public TextBox(TextInputState state, IInputHelper inputHelper)
        {
            this.state = state;
            this.inputHelper = inputHelper;
        }

        private IGuiComponent CreateInner(
            TextInputState state,
            IInputHelper inputHelper
        )
        {
            var background = Game1.content.Load<Texture2D>(@"LooseSprites\textBox");
            return new TextInputComponent(state, inputHelper)
                {
                    HighlightedTextBackgroundColor = new(Color.DeepSkyBlue, 0.5f),
                    CursorColor = new(Color.Black, 0.75f),
                }.WithPadding(16, 6, 6, 8)
                .WithBackground(new TextureComponent(background));
        }
    }
}

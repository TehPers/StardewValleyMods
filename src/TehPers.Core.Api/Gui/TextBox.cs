using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A standard text box.
    /// </summary>
    public class TextBox : WrapperComponent
    {
        /// <summary>
        /// Creates a new text box.
        /// </summary>
        /// <param name="textState">The state of the text input.</param>
        /// <param name="inputHelper">The input helper.</param>
        public TextBox(TextInput.State textState, IInputHelper inputHelper)
            : base(TextBox.CreateInner(textState, inputHelper))
        {
        }

        private static IGuiComponent CreateInner(
            TextInput.State textState,
            IInputHelper inputHelper
        )
        {
            var background = Game1.content.Load<Texture2D>(@"LooseSprites\textBox");
            return new TextInput(textState, inputHelper)
                {
                    HighlightedTextBackgroundColor = new(Color.DeepSkyBlue, 0.5f),
                    CursorColor = new(Color.Black, 0.75f),
                }.WithPadding(16, 6, 6, 8)
                .WithBackground(new StretchedTexture(background));
        }
    }
}

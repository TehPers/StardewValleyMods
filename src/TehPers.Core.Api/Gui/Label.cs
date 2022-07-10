using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A text label in a GUI.
    /// </summary>
    /// <param name="Text">The text on this label.</param>
    internal record Label(string Text) : IGuiComponent
    {
        /// <summary>
        /// The font to draw this label with.
        /// </summary>
        public SpriteFont Font { get; init; } = Game1.smallFont;

        /// <summary>
        /// The color to tint the text.
        /// </summary>
        public Color Color { get; init; } = Game1.textColor;

        /// <summary>
        /// The scale of the text.
        /// </summary>
        public Vector2 Scale { get; init; } = Vector2.One;

        /// <summary>
        /// The effects to apply to the font's sprites.
        /// </summary>
        public SpriteEffects SpriteEffects { get; init; } = SpriteEffects.None;

        /// <summary>
        /// The layer depth of the text.
        /// </summary>
        public float LayerDepth { get; init; } = 0;
        
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var size = this.Font.MeasureString(this.Text);
            return new()
            {
                MinSize = new(size),
                MaxSize = new(size),
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            e.Draw(
                batch => batch.DrawString(
                    this.Font,
                    this.Text,
                    new(bounds.X, bounds.Y),
                    this.Color,
                    0,
                    Vector2.Zero,
                    this.Scale,
                    this.SpriteEffects,
                    this.LayerDepth
                )
            );
        }
    }
}

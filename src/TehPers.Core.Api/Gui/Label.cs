using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A text label in a GUI.
    /// </summary>
    /// <param name="Text">The text on this label.</param>
    /// <param name="Font">The font to draw this label with.</param>
    public record Label(string Text, SpriteFont Font) : BaseGuiComponent
    {
        /// <summary>
        /// The color to tint the text.
        /// </summary>
        public Color Color { get; init; } = Color.White;

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
        public override GuiConstraints Constraints => Label.GetConstraints(this.Font, this.Text);

        private static GuiConstraints GetConstraints(SpriteFont font, string text)
        {
            var size = font.MeasureString(text);
            return new()
            {
                MinSize = new(size),
                MaxSize = new(size),
            };
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch batch, Rectangle bounds)
        {
            base.Draw(batch, bounds);

            batch.DrawString(
                this.Font,
                this.Text,
                new(bounds.X, bounds.Y),
                this.Color,
                0,
                Vector2.Zero,
                this.Scale,
                this.SpriteEffects,
                this.LayerDepth
            );
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            newComponent = default;
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A text label in a GUI.
    /// </summary>
    /// <param name="Text">The text on this label.</param>
    /// <param name="Font">The font to draw this label with.</param>
    public record Label(string Text, SpriteFont Font) : IGuiComponent
    {
        /// <summary>
        /// The color to tint the text.
        /// </summary>
        public Color Color { get; init; } = Color.Black;

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

        /// <summary>
        /// Creates a new <see cref="Label"/> component.
        /// </summary>
        /// <param name="text">The text on this label.</param>
        /// <param name="font">The font to draw this label with.</param>
        /// <param name="color">The color to tint the text.</param>
        /// <param name="scale">The scale of the text.</param>
        /// <param name="spriteEffects">The effects to apply to the font's sprites.</param>
        /// <param name="layerDepth">The layer depth of the text.</param>
        public Label(
            string text,
            SpriteFont font,
            Color? color = default,
            Vector2? scale = default,
            SpriteEffects? spriteEffects = default,
            float? layerDepth = default
        )
            : this(text, font)
        {
            this.Color = color ?? this.Color;
            this.Scale = scale ?? this.Scale;
            this.SpriteEffects = spriteEffects ?? this.SpriteEffects;
            this.LayerDepth = layerDepth ?? this.LayerDepth;
        }

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

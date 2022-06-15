using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A text label in a GUI.
    /// </summary>
    public class Label : IGuiComponent<Label.State>
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

        /// <summary>
        /// The text on this label.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The font to draw this label with.
        /// </summary>
        public SpriteFont Font { get; }

        /// <summary>
        /// A text label in a GUI.
        /// </summary>
        /// <param name="text">The text on this label.</param>
        /// <param name="font">The font to draw this label with.</param>
        public Label(string text, SpriteFont font)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
            this.Font = font ?? throw new ArgumentNullException(nameof(font));
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
        public State Initialize(Rectangle bounds)
        {
            return new(bounds);
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return new(bounds);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
            batch.DrawString(
                this.Font,
                this.Text,
                new(state.Bounds.X, state.Bounds.Y),
                this.Color,
                0,
                Vector2.Zero,
                this.Scale,
                this.SpriteEffects,
                this.LayerDepth
            );
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            nextState = default;
            return false;
        }

        /// <summary>
        /// The state for a <see cref="Label"/>.
        /// </summary>
        /// <param name="Bounds">The bounds the label is being rendered in.</param>
        public record State(Rectangle Bounds);
    }
}

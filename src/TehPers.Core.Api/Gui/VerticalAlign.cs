using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Vertically aligns a component. This removes any maximum height constraint.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Alignment">The type of alignment to apply.</param>
    public record VerticalAlign<TState>
        (IGuiComponent<TState> Inner, VerticalAlignment Alignment) : IGuiComponent<TState>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = innerConstraints.MaxSize with
                {
                    Height = null,
                },
            };
        }

        private Rectangle GetInnerBounds(Rectangle bounds)
        {
            // Calculate inner width
            var innerConstraints = this.Inner.GetConstraints();
            // Calculate inner width
            var innerHeight = innerConstraints.MaxSize.Height switch
            {
                null => bounds.Height,
                { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
            };

            // Calculate y position
            var y = this.Alignment switch
            {
                VerticalAlignment.Top => bounds.Top,
                VerticalAlignment.Center => bounds.Top + (bounds.Height - innerHeight) / 2,
                VerticalAlignment.Bottom => bounds.Bottom - innerHeight,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.Alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            return new(bounds.X, y, bounds.Width, innerHeight);
        }

        /// <inheritdoc />
        public TState Initialize(Rectangle bounds)
        {
            return this.Inner.Initialize(this.GetInnerBounds(bounds));
        }

        /// <inheritdoc />
        public TState Reposition(TState state, Rectangle bounds)
        {
            return this.Inner.Reposition(state, this.GetInnerBounds(bounds));
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, TState state)
        {
            this.Inner.Draw(batch, state);
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, TState state, [NotNullWhen(true)] out TState? nextState)
        {
            return this.Inner.Update(e, state, out nextState);
        }
    }
}

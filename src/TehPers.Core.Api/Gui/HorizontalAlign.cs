using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Horizontally aligns a component. This removes any maximum width constraint.
    /// </summary>
    /// <typeparam name="TState">The type of the inner component's state.</typeparam>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Alignment">The type of alignment to apply.</param>
    public record HorizontalAlign<TState>(
        IGuiComponent<TState> Inner,
        HorizontalAlignment Alignment
    ) : IGuiComponent<TState>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = innerConstraints.MaxSize with
                {
                    Width = null,
                },
            };
        }

        private Rectangle GetInnerBounds(Rectangle bounds)
        {
            // Calculate inner width
            var innerConstraints = this.Inner.GetConstraints();
            var innerWidth = innerConstraints.MaxSize.Width switch
            {
                null => bounds.Width,
                { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
            };

            // Calculate x position
            var x = this.Alignment switch
            {
                HorizontalAlignment.Left => bounds.Left,
                HorizontalAlignment.Center => bounds.Left + (bounds.Width - innerWidth) / 2,
                HorizontalAlignment.Right => bounds.Right - innerWidth,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.Alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            return new(x, bounds.Y, innerWidth, bounds.Height);
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

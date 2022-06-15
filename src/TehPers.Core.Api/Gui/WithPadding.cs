using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Adds padding to a component.
    /// </summary>
    /// <typeparam name="TState">The type of the inner component's state.</typeparam>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Left">Padding to add to the left side.</param>
    /// <param name="Right">Padding to add to the right side.</param>
    /// <param name="Top">Padding to add to the top.</param>
    /// <param name="Bottom">Padding to add to the bottom.</param>
    public record WithPadding<TState>(
        IGuiComponent<TState> Inner,
        float Left,
        float Right,
        float Top,
        float Bottom
    ) : IGuiComponent<TState>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MinSize = new(
                    innerConstraints.MinSize.Width + this.Left + this.Right,
                    innerConstraints.MinSize.Height + this.Top + this.Bottom
                ),
                MaxSize = new(
                    innerConstraints.MaxSize.Width switch
                    {
                        null => null,
                        { } w => w + this.Left + this.Right
                    },
                    innerConstraints.MaxSize.Height switch
                    {
                        null => null,
                        { } h => h + this.Top + this.Bottom
                    }
                ),
            };
        }

        private Rectangle GetInnerBounds(Rectangle bounds)
        {
            return new(
                (int)(bounds.X + this.Left),
                (int)(bounds.Y + this.Top),
                (int)Math.Max(0, Math.Ceiling(bounds.Width - this.Left - this.Right)),
                (int)Math.Max(0, Math.Ceiling(bounds.Height - this.Top - this.Bottom))
            );
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

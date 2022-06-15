using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Shrinks a GUI component to its minimum size.
    /// </summary>
    /// <typeparam name="TState">The type of the inner component's state.</typeparam>
    /// <param name="Inner">The inner component.</param>
    public record Shrink<TState>(IGuiComponent<TState> Inner) : IGuiComponent<TState>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = new(innerConstraints.MinSize),
            };
        }

        /// <inheritdoc />
        public TState Initialize(Rectangle bounds)
        {
            return this.Inner.Initialize(bounds);
        }

        /// <inheritdoc />
        public TState Reposition(TState state, Rectangle bounds)
        {
            return this.Inner.Reposition(state, bounds);
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

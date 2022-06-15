using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A component with simple functionality.
    /// </summary>
    public class SimpleComponent : IGuiComponent<SimpleComponent.State>
    {
        private readonly GuiConstraints constraints;
        private readonly Action<SpriteBatch, Rectangle> draw;

        /// <summary>
        /// Creates a new <see cref="SimpleComponent"/>.
        /// </summary>
        /// <param name="constraints">The constraints on the component.</param>
        /// <param name="draw">A callback which draws the component.</param>
        public SimpleComponent(GuiConstraints constraints, Action<SpriteBatch, Rectangle> draw)
        {
            this.constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
            this.draw = draw ?? throw new ArgumentNullException(nameof(draw));
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.constraints;
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
            this.draw(batch, state.Bounds);
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            nextState = default;
            return false;
        }

        /// <summary>
        /// The state for a <see cref="SimpleComponent"/>.
        /// </summary>
        public class State
        {
            internal Rectangle Bounds { get; }

            internal State(Rectangle bounds)
            {
                this.Bounds = bounds;
            }
        }
    }
}

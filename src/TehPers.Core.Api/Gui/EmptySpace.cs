using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Empty space in a GUI.
    /// </summary>
    public record EmptySpace : IGuiComponent<EmptySpace.State>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return new();
        }

        /// <inheritdoc />
        public State Initialize(Rectangle bounds)
        {
            return State.Instance;
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return state;
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            nextState = default;
            return false;
        }

        /// <summary>
        /// The state of an <see cref="EmptySpace"/> component.
        /// </summary>
        public sealed class State
        {
            /// <summary>
            /// An instance of the state.
            /// </summary>
            internal static State Instance { get; } = new();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A GUI component that can be drawn to the screen.
    /// </summary>
    public interface IGuiComponent<TState>
    {
        /// <summary>
        /// Gets the constraints on how this component should be rendered.
        /// </summary>
        /// <returns>The constraints on how this component should be rendered.</returns>
        GuiConstraints GetConstraints();

        /// <summary>
        /// Initializes the state of this component.
        /// </summary>
        /// <param name="bounds">The bounds of this component.</param>
        /// <returns>The component's initial state.</returns>
        TState Initialize(Rectangle bounds);

        /// <summary>
        /// Repositions this component and updates its state.
        /// </summary>
        /// <param name="state">The state of this component.</param>
        /// <param name="bounds">The area this component should be drawn in.</param>
        /// <returns>The updated state of this component.</returns>
        TState Reposition(TState state, Rectangle bounds);

        /// <summary>
        /// Draws this component to the screen. This method does not draw the component's children.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="state">The state of this component.</param>
        void Draw(SpriteBatch batch, TState state);

        /// <summary>
        /// Updates this component in response to an event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="state">The state of this component.</param>
        /// <param name="nextState">The next state of this component.</param>
        /// <returns>Whether this component was updated.</returns>
        bool Update(GuiEvent e, TState state, [NotNullWhen(true)] out TState? nextState);
    }
}

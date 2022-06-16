using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A GUI component that can be drawn to the screen.
    /// </summary>
    public interface IGuiComponent : IGuiComponent<Unit>
    {
        /// <summary>
        /// Handles a UI event. This is where the component draws to the screen, handles input, and
        /// does whatever else it needs to do.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <returns>Whether this component was updated.</returns>
        new void Handle(GuiEvent e, Rectangle bounds);

        /// <inheritdoc />
        Unit IGuiComponent<Unit>.Handle(GuiEvent e, Rectangle bounds)
        {
            this.Handle(e, bounds);
            return default;
        }
    }

    /// <summary>
    /// A GUI component that can be drawn to the screen.
    /// </summary>
    public interface IGuiComponent<out TResponse>
    {
        /// <summary>
        /// Gets the constraints on how this component should be rendered.
        /// </summary>
        /// <returns>The constraints on how this component should be rendered.</returns>
        GuiConstraints GetConstraints();

        /// <summary>
        /// Handles a UI event. This is where the component draws to the screen, handles input, and
        /// does whatever else it needs to do.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <returns>The component's response.</returns>
        TResponse Handle(GuiEvent e, Rectangle bounds);
    }
}

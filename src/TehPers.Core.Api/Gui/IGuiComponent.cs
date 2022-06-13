using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A GUI component that can be drawn to the screen.
    /// </summary>
    public interface IGuiComponent
    {
        /// <summary>
        /// Gets the constraints on how this component should be rendered.
        /// </summary>
        GuiConstraints Constraints { get; }

        /// <summary>
        /// Calculates the layouts of this component and all its children.
        /// </summary>
        /// <param name="bounds">The area reserved for this component.</param>
        /// <param name="layouts"></param>
        /// <returns>The layouts of this component and all its children.</returns>
        void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts);

        /// <summary>
        /// Draws this component to the screen. This method does not draw the component's children.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="bounds">The area this component should be drawn in.</param>
        void Draw(SpriteBatch batch, Rectangle bounds);

        /// <summary>
        /// Updates this component in response to an event if necessary.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="componentBounds">The bounds for each component.</param>
        /// <param name="newComponent">The updated component.</param>
        /// <returns>Whether this component was updated.</returns>
        bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        );
    }
}

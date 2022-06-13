using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A component with its layout.
    /// </summary>
    public class ComponentLayout
    {
        /// <summary>
        /// The component.
        /// </summary>
        public IGuiComponent Component { get; }

        /// <summary>
        /// The bounds of the component.
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Creates a <see cref="ComponentLayout"/> from its parts.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="bounds">The bounds of the component.</param>
        public ComponentLayout(IGuiComponent component, Rectangle bounds)
        {
            this.Component = component ?? throw new ArgumentNullException(nameof(component));
            this.Bounds = bounds;
        }

        /// <summary>
        /// Draws the contained component.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        public void Draw(SpriteBatch batch)
        {
            this.Component.Draw(batch, this.Bounds);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A component with simple functionality.
    /// </summary>
    /// <param name="Constraints">The constraints on the component.</param>
    /// <param name="Draw">A callback which draws the component.</param>
    public record SimpleComponent(
        GuiConstraints Constraints,
        Action<SpriteBatch, Rectangle> Draw
    ) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Constraints;
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            e.Draw(batch => this.Draw(batch, bounds));
        }
    }
}

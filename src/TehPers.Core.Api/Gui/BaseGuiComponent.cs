using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A base GUI component that can be drawn to the screen with some default functionality.
    /// </summary>
    public abstract record BaseGuiComponent : IGuiComponent
    {
        /// <inheritdoc />
        public abstract GuiConstraints Constraints { get; }

        /// <inheritdoc />
        public virtual void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            layouts.Add(new(this, bounds));
        }

        /// <inheritdoc />
        public virtual void Draw(SpriteBatch batch, Rectangle bounds)
        {
        }

        /// <inheritdoc />
        public abstract bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        );
    }
}

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Shrinks a GUI component to its minimum size.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    public record Shrink(IGuiComponent Inner) : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints =>
            this.Inner.Constraints with
            {
                MaxSize = new(this.Inner.Constraints.MinSize),
            };

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.Inner.CalculateLayouts(bounds, layouts);
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            if (this.Inner.Update(e, componentBounds, out var newInner))
            {
                newComponent = this with {Inner = newInner};
                return true;
            }

            newComponent = default;
            return false;
        }
    }
}

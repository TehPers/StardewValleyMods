using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Fully aligns a component along both axes. This removes any maximum size constraint.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="HorizontalAlignment">The type of horizontal alignment to apply.</param>
    /// <param name="VerticalAlignment">The type of vertical alignment to apply.</param>
    public record FullAlign(
        IGuiComponent Inner,
        HorizontalAlignment HorizontalAlignment,
        VerticalAlignment VerticalAlignment
    ) : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints => this.InnerAligned.Constraints;

        private IGuiComponent InnerAligned { get; init; } = new VerticalAlign(
            new HorizontalAlign(Inner, HorizontalAlignment),
            VerticalAlignment
        );

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.InnerAligned.CalculateLayouts(bounds, layouts);
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            if (this.InnerAligned.Update(e, componentBounds, out var newInner))
            {
                newComponent = this with {InnerAligned = newInner};
                return true;
            }

            newComponent = default;
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Vertically aligns a component. This removes any maximum height constraint.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Alignment">The type of alignment to apply.</param>
    public record VerticalAlign(IGuiComponent Inner, VerticalAlignment Alignment) : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints =>
            this.Inner.Constraints with
            {
                MaxSize = this.Inner.Constraints.MaxSize with
                {
                    Height = null,
                },
            };

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);

            // Calculate inner width
            var innerHeight = this.Inner.Constraints.MaxSize.Height switch
            {
                null => bounds.Height,
                { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
            };

            // Calculate y position
            var y = this.Alignment switch
            {
                VerticalAlignment.Top => bounds.Top,
                VerticalAlignment.Center => bounds.Top + (bounds.Height - innerHeight) / 2,
                VerticalAlignment.Bottom => bounds.Bottom - innerHeight,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.Alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            this.Inner.CalculateLayouts(new(bounds.X, y, bounds.Width, innerHeight), layouts);
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

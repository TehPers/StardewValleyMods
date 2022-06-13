using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Horizontally aligns a component. This removes any maximum width constraint.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Alignment">The type of alignment to apply.</param>
    public record HorizontalAlign
        (IGuiComponent Inner, HorizontalAlignment Alignment) : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints =>
            this.Inner.Constraints with
            {
                MaxSize = this.Inner.Constraints.MaxSize with
                {
                    Width = null,
                },
            };

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);

            // Calculate inner width
            var innerWidth = this.Inner.Constraints.MaxSize.Width switch
            {
                null => bounds.Width,
                { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
            };

            // Calculate x position
            var x = this.Alignment switch
            {
                HorizontalAlignment.Left => bounds.Left,
                HorizontalAlignment.Center => bounds.Left + (bounds.Width - innerWidth) / 2,
                HorizontalAlignment.Right => bounds.Right - innerWidth,
                _ => throw new InvalidOperationException(
                    $"{nameof(this.Alignment)} has an invalid value"
                ),
            };

            // Layout inner component
            this.Inner.CalculateLayouts(new(x, bounds.Y, innerWidth, bounds.Height), layouts);
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

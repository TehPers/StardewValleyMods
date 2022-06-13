using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Adds padding to a component.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Left">Padding to add to the left side.</param>
    /// <param name="Right">Padding to add to the right side.</param>
    /// <param name="Top">Padding to add to the top.</param>
    /// <param name="Bottom">Padding to add to the bottom.</param>
    public record WithPadding(
        IGuiComponent Inner,
        float Left,
        float Right,
        float Top,
        float Bottom
    ) : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints =>
            this.Inner.Constraints with
            {
                MinSize = new(
                    this.Inner.Constraints.MinSize.Width + this.Left + this.Right,
                    this.Inner.Constraints.MinSize.Height + this.Top + this.Bottom
                ),
                MaxSize = new(
                    this.Inner.Constraints.MaxSize.Width switch
                    {
                        null => null,
                        { } w => w + this.Left + this.Right
                    },
                    this.Inner.Constraints.MaxSize.Height switch
                    {
                        null => null,
                        { } h => h + this.Top + this.Bottom
                    }
                ),
            };

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.Inner.CalculateLayouts(
                new(
                    (int)(bounds.X + this.Left),
                    (int)(bounds.Y + this.Top),
                    (int)Math.Max(0, Math.Ceiling(bounds.Width - this.Left - this.Right)),
                    (int)Math.Max(0, Math.Ceiling(bounds.Height - this.Top - this.Bottom))
                ),
                layouts
            );
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

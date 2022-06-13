using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Draws a component with a background.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="Background">The background component.</param>
    public record WithBackground(IGuiComponent Inner, IGuiComponent Background) : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints => new()
        {
            MinSize = new(
                Math.Max(
                    this.Background.Constraints.MinSize.Width,
                    this.Inner.Constraints.MinSize.Width
                ),
                Math.Max(
                    this.Background.Constraints.MinSize.Height,
                    this.Inner.Constraints.MinSize.Height
                )
            ),
            MaxSize = new(
                (this.Background.Constraints.MaxSize.Width,
                        this.Inner.Constraints.MaxSize.Width) switch
                    {
                        (null, var w) => w,
                        (var w, null) => w,
                        ({ } w1, { } w2) => Math.Min(w1, w2),
                    },
                (this.Background.Constraints.MaxSize.Height,
                        this.Inner.Constraints.MaxSize.Height) switch
                    {
                        (null, var h) => h,
                        (var h, null) => h,
                        ({ } h1, { } h2) => Math.Min(h1, h2),
                    }
            ),
            AllowBuffer = this.Background.Constraints.AllowBuffer
                && this.Inner.Constraints.AllowBuffer,
        };

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.Background.CalculateLayouts(bounds, layouts);
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

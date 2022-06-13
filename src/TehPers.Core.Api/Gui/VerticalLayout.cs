using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A vertical layout container. Components are rendered vertically along a single column.
    /// </summary>
    /// <param name="Components">The components in this layout.</param>
    public record VerticalLayout(ImmutableList<IGuiComponent> Components) : BaseGuiComponent
    {
        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <returns>A vertical layout containing the given components.</returns>
        public static VerticalLayout Of(params IGuiComponent[] components)
        {
            return new(components.ToImmutableList());
        }

        /// <inheritdoc />
        public override GuiConstraints Constraints =>
            this.Components.Aggregate(
                new GuiConstraints {MaxSize = new(null, 0)},
                (prev, component) => new()
                {
                    MinSize = new(
                        Math.Max(prev.MinSize.Width, component.Constraints.MinSize.Width),
                        prev.MinSize.Height + component.Constraints.MinSize.Height
                    ),
                    MaxSize = new(
                        (prev.MaxSize.Width, component.Constraints.MaxSize.Width) switch
                        {
                            (null, var w) => w,
                            (var w, null) => w,
                            ({ } w1, { } w2) => Math.Min(w1, w2),
                        },
                        (prev.MaxSize.Height, component.Constraints.MaxSize.Height) switch
                        {
                            (null, _) => null,
                            (_, null) => null,
                            ({ } h1, { } h2) => h1 + h2,
                        }
                    )
                }
            );

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);

            // Get excess vertical space
            var sizedComponents = this.Components.Select(
                    component => new SizedComponent(component, component.Constraints)
                )
                .ToList();
            var excessHeight = sizedComponents.Aggregate(
                (float)bounds.Height,
                (excess, cur) => excess - cur.MinHeight
            );
            excessHeight = Math.Max(0, excessHeight);

            // Scale excess across all components
            while (excessHeight > 0)
            {
                var remainingComponents =
                    sizedComponents.Where(c => c.RemainingHeight is not <= 0).ToList();
                if (!remainingComponents.Any())
                {
                    break;
                }

                var minAddedHeight = remainingComponents.Min(
                    c => c.RemainingHeight ?? excessHeight
                );
                var addedHeight = Math.Min(
                    excessHeight / remainingComponents.Count,
                    minAddedHeight
                );
                foreach (var c in remainingComponents)
                {
                    c.AdditionalHeight += addedHeight;
                }

                excessHeight -= addedHeight * remainingComponents.Count;
            }

            // Layout components, using up excess space if able
            foreach (var sizedComponent in sizedComponents)
            {
                // Calculate width and x-position
                var width = sizedComponent.Constraints.MaxSize.Width switch
                {
                    null => bounds.Width,
                    { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
                };

                // Calculate height
                var height = (int)Math.Ceiling(
                    sizedComponent.MinHeight + sizedComponent.AdditionalHeight
                );
                sizedComponent.Component.CalculateLayouts(
                    new(bounds.X, bounds.Y, width, height),
                    layouts
                );

                // Update remaining area
                bounds = new(
                    bounds.X,
                    bounds.Y + height,
                    width,
                    Math.Max(0, bounds.Height - height)
                );
            }
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            // Update all children
            var changed = false;
            var builder = this.Components.ToBuilder();
            for (var i = 0; i < this.Components.Count; i++)
            {
                if (!this.Components[i].Update(e, componentBounds, out var newChild))
                {
                    continue;
                }

                changed = true;
                builder[i] = newChild;
            }

            // Check if any children changed
            if (changed)
            {
                newComponent = this with {Components = builder.ToImmutable()};
                return true;
            }

            newComponent = default;
            return false;
        }

        private class SizedComponent
        {
            public IGuiComponent Component { get; }
            public GuiConstraints Constraints { get; }
            public float AdditionalHeight { get; set; }

            public float MinHeight => this.Constraints.MinSize.Height;

            public float? RemainingHeight => this.Constraints.MaxSize.Height switch
            {
                null => null,
                { } maxHeight => maxHeight - this.Constraints.MinSize.Height - this.AdditionalHeight
            };

            public SizedComponent(IGuiComponent component, GuiConstraints constraints)
            {
                this.Component = component;
                this.Constraints = constraints;
                this.AdditionalHeight = 0;
            }
        }
    }
}

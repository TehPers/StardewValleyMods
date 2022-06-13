using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A horizontal layout container. Components are rendered horizontally along a single column.
    /// </summary>
    /// <param name="Components">The components in this layout.</param>
    public record HorizontalLayout(ImmutableList<IGuiComponent> Components) : BaseGuiComponent
    {
        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <returns>A horizontal layout containing the given components.</returns>
        public static HorizontalLayout Of(params IGuiComponent[] components)
        {
            return new(components.ToImmutableList());
        }

        /// <inheritdoc />
        public override GuiConstraints Constraints =>
            this.Components.Aggregate(
            new GuiConstraints {MaxSize = new(0, null)},
            (prev, component) => new()
            {
                MinSize = new(
                    prev.MinSize.Width + component.Constraints.MinSize.Width,
                    Math.Max(prev.MinSize.Height, component.Constraints.MinSize.Height)
                ),
                MaxSize = new(
                    (prev.MaxSize.Width, component.Constraints.MaxSize.Width) switch
                    {
                        (null, _) => null,
                        (_, null) => null,
                        ({ } w1, { } w2) => w1 + w2,
                    },
                    (prev.MaxSize.Height, component.Constraints.MaxSize.Height) switch
                    {
                        (null, var h) => h,
                        (var h, null) => h,
                        ({ } h1, { } h2) => Math.Min(h1, h2),
                    }
                )
            }
        );

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);

            // Get excess horizontal space
            var sizedComponents = this.Components.Select(
                    component => new SizedComponent(component, component.Constraints)
                )
                .ToList();
            var excessWidth = sizedComponents.Aggregate(
                (float)bounds.Width,
                (excess, cur) => excess - cur.MinWidth
            );
            excessWidth = Math.Max(0, excessWidth);

            // Scale excess across all components
            while (excessWidth > 0)
            {
                var remainingComponents =
                    sizedComponents.Where(c => c.RemainingWidth is not <= 0).ToList();
                if (!remainingComponents.Any())
                {
                    break;
                }

                var minAddedWidth = remainingComponents.Min(c => c.RemainingWidth ?? excessWidth);
                var addedWidth = Math.Min(excessWidth / remainingComponents.Count, minAddedWidth);
                foreach (var c in remainingComponents)
                {
                    c.AdditionalWidth += addedWidth;
                }

                excessWidth -= addedWidth * remainingComponents.Count;
            }

            // Layout components, using up excess space if able
            foreach (var sizedComponent in sizedComponents)
            {
                // Calculate height and y-position
                var height = sizedComponent.Constraints.MaxSize.Height switch
                {
                    null => bounds.Height,
                    { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
                };

                // Calculate width
                var width = (int)Math.Ceiling(
                    sizedComponent.MinWidth + sizedComponent.AdditionalWidth
                );
                sizedComponent.Component.CalculateLayouts(
                    new(bounds.X, bounds.Y, width, height),
                    layouts
                );

                // Update remaining area
                bounds = new(bounds.X + width, bounds.Y, Math.Max(0, bounds.Width - width), height);
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
            public float AdditionalWidth { get; set; }

            public float MinWidth => this.Constraints.MinSize.Width;

            public float? RemainingWidth => this.Constraints.MaxSize.Width switch
            {
                null => null,
                { } maxWidth => maxWidth - this.Constraints.MinSize.Width - this.AdditionalWidth
            };

            public SizedComponent(IGuiComponent component, GuiConstraints constraints)
            {
                this.Component = component;
                this.Constraints = constraints;
                this.AdditionalWidth = 0;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// A vertical layout container. Components are rendered vertically along a single column.
    /// </summary>
    /// <param name="Components">The inner components.</param>
    public record VerticalLayout(ImmutableArray<IGuiComponent> Components) : IGuiComponent
    {
        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public VerticalLayout(params IGuiComponent[] components)
            : this(components.ToImmutableArray())
        {
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public VerticalLayout(IEnumerable<IGuiComponent> components)
            : this(components.ToImmutableArray())
        {
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Components.Aggregate(
                new GuiConstraints {MaxSize = new(null, 0)},
                (prev, component) =>
                {
                    var constraints = component.GetConstraints();
                    return new()
                    {
                        MinSize = new(
                            Math.Max(prev.MinSize.Width, constraints.MinSize.Width),
                            prev.MinSize.Height + constraints.MinSize.Height
                        ),
                        MaxSize = new(
                            (prev.MaxSize.Width, constraints.MaxSize.Width) switch
                            {
                                (null, var w) => w,
                                (var w, null) => w,
                                ({ } w1, { } w2) => Math.Min(w1, w2),
                            },
                            (prev.MaxSize.Height, constraints.MaxSize.Height) switch
                            {
                                (null, _) => null,
                                (_, null) => null,
                                ({ } h1, { } h2) => h1 + h2,
                            }
                        )
                    };
                }
            );
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            // Get excess vertical space
            var sizedComponents = this.Components.Select(
                    component => new SizedComponent(component, component.GetConstraints())
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
                sizedComponent.Component.Handle(e, new(bounds.X, bounds.Y, width, height));

                // Update remaining area
                bounds = new(
                    bounds.X,
                    bounds.Y + height,
                    width,
                    Math.Max(0, bounds.Height - height)
                );
            }
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout Of(params IGuiComponent[] components)
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout Of(IEnumerable<IGuiComponent> components)
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout Build(Action<ILayoutBuilder> addComponents)
        {
            var builder = new Builder(null);
            addComponents(builder);
            return builder.Build();
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components. The components are
        /// automatically aligned as they're added to the layout.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <param name="defaultAlignment">The default row alignment.</param>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout BuildAligned(
            Action<ILayoutBuilder> addComponents,
            HorizontalAlignment defaultAlignment = HorizontalAlignment.Left
        )
        {
            var builder = new Builder(defaultAlignment);
            addComponents(builder);
            return builder.Build();
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

        private class Builder : ILayoutBuilder
        {
            private readonly HorizontalAlignment? defaultAlignment;
            private readonly List<IGuiComponent> components;

            public Builder(HorizontalAlignment? defaultAlignment)
            {
                this.defaultAlignment = defaultAlignment;
                this.components = new();
            }

            public void Add(IGuiComponent component)
            {
                if (this.defaultAlignment is { } defaultAlignment)
                {
                    component = component.Aligned(defaultAlignment);
                }

                this.components.Add(component);
            }

            public VerticalLayout Build()
            {
                return new(this.components.ToImmutableArray());
            }

            IGuiComponent ILayoutBuilder.Build()
            {
                return this.Build();
            }
        }
    }
}

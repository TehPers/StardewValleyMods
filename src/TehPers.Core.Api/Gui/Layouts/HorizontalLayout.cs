using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// A horizontal layout container. Components are rendered horizontally along a single row.
    /// </summary>
    /// <param name="Components">The components in this layout.</param>
    public record HorizontalLayout(ImmutableArray<IGuiComponent> Components) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Components.Aggregate(
                new GuiConstraints {MaxSize = new(0, null)},
                (prev, component) =>
                {
                    var innerConstraints = component.GetConstraints();
                    return new()
                    {
                        MinSize = new(
                            prev.MinSize.Width + innerConstraints.MinSize.Width,
                            Math.Max(prev.MinSize.Height, innerConstraints.MinSize.Height)
                        ),
                        MaxSize = new(
                            (prev.MaxSize.Width, innerConstraints.MaxSize.Width) switch
                            {
                                (null, _) => null,
                                (_, null) => null,
                                ({ } w1, { } w2) => w1 + w2,
                            },
                            (prev.MaxSize.Height, innerConstraints.MaxSize.Height) switch
                            {
                                (null, var h) => h,
                                (var h, null) => h,
                                ({ } h1, { } h2) => Math.Min(h1, h2),
                            }
                        )
                    };
                }
            );
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            // Get excess horizontal space
            var sizedComponents = this.Components.Select(
                    component => new SizedComponent(component, component.GetConstraints())
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
                sizedComponent.Component.Handle(e, new(bounds.X, bounds.Y, width, height));

                // Update remaining area
                bounds = new(bounds.X + width, bounds.Y, Math.Max(0, bounds.Width - width), height);
            }
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout Of(params IGuiComponent[] components)
        {
            return new(components.ToImmutableArray());
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout Of(IEnumerable<IGuiComponent> components)
        {
            return new(components.ToImmutableArray());
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <param name="defaultAlignment">The default column alignment.</param>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout Build(
            Action<ILayoutBuilder> addComponents,
            VerticalAlignment defaultAlignment = VerticalAlignment.Top
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

        private class Builder : ILayoutBuilder
        {
            private readonly VerticalAlignment defaultAlignment;
            private readonly List<IGuiComponent> components;

            public Builder(VerticalAlignment defaultAlignment)
            {
                this.defaultAlignment = defaultAlignment;
                this.components = new();
            }

            public void Add(IGuiComponent component)
            {
                this.components.Add(component.Aligned(this.defaultAlignment));
            }

            public HorizontalLayout Build()
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

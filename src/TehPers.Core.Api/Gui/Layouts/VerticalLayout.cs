using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// Utility methods for <see cref="VerticalLayout{TResponse}"/>.
    /// </summary>
    public static class VerticalLayout
    {
        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout<TResponse> Of<TResponse>(
            params IGuiComponent<TResponse>[] components
        )
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout<TResponse> Of<TResponse>(
            IEnumerable<IGuiComponent<TResponse>> components
        )
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout<Unit> Build(
            Action<ILayoutBuilder<Unit, VerticalLayout<Unit>>> addComponents
        )
        {
            return VerticalLayout.Build<Unit>(addComponents);
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout<TResponse> Build<TResponse>(
            Action<ILayoutBuilder<TResponse, VerticalLayout<TResponse>>> addComponents
        )
        {
            var builder = new Builder<TResponse>();
            addComponents(builder);
            return builder.Build();
        }

        /// <summary>
        /// A vertical layout builder.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        private class Builder<TResponse> : ILayoutBuilder<TResponse, VerticalLayout<TResponse>>
        {
            private readonly List<IGuiComponent<TResponse>> components;

            /// <summary>
            /// Creates a new vertical layout builder.
            /// </summary>
            public Builder()
            {
                this.components = new();
            }

            /// <summary>
            /// Adds a new component to this layout.
            /// </summary>
            /// <param name="component">The component to add.</param>
            public void Add(IGuiComponent<TResponse> component)
            {
                this.components.Add(component);
            }

            /// <summary>
            /// Builds the layout from this builder.
            /// </summary>
            /// <returns>The vertical layout.</returns>
            public VerticalLayout<TResponse> Build()
            {
                return new(this.components.ToImmutableArray());
            }
        }
    }

    /// <summary>
    /// A vertical layout container. Components are rendered vertically along a single column.
    /// </summary>
    /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
    /// <param name="Components">The inner components.</param>
    public record VerticalLayout<TResponse>
        (ImmutableArray<IGuiComponent<TResponse>> Components) : IGuiComponent<
            IEnumerable<VerticalLayout<TResponse>.ResponseItem>>
    {
        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public VerticalLayout(params IGuiComponent<TResponse>[] components)
            : this(components.ToImmutableArray())
        {
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public VerticalLayout(IEnumerable<IGuiComponent<TResponse>> components)
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
        public IEnumerable<ResponseItem> Handle(GuiEvent e, Rectangle bounds)
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
            var responses = new List<ResponseItem>(this.Components.Length);
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
                var response = sizedComponent.Component.Handle(
                    e,
                    new(bounds.X, bounds.Y, width, height)
                );
                responses.Add(new(sizedComponent.Component, response));

                // Update remaining area
                bounds = new(
                    bounds.X,
                    bounds.Y + height,
                    width,
                    Math.Max(0, bounds.Height - height)
                );
            }

            return responses;
        }

        /// <summary>
        /// The type of response of a <see cref="VerticalLayout{TResponse}"/>.
        /// </summary>
        /// <param name="Component">The component that responded.</param>
        /// <param name="Response">The component's response.</param>
        public record ResponseItem(IGuiComponent<TResponse> Component, TResponse Response);

        private class SizedComponent
        {
            public IGuiComponent<TResponse> Component { get; }
            public GuiConstraints Constraints { get; }
            public float AdditionalHeight { get; set; }

            public float MinHeight => this.Constraints.MinSize.Height;

            public float? RemainingHeight => this.Constraints.MaxSize.Height switch
            {
                null => null,
                { } maxHeight => maxHeight - this.Constraints.MinSize.Height - this.AdditionalHeight
            };

            public SizedComponent(IGuiComponent<TResponse> component, GuiConstraints constraints)
            {
                this.Component = component;
                this.Constraints = constraints;
                this.AdditionalHeight = 0;
            }
        }
    }
}

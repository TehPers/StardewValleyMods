using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// Utility methods for <see cref="HorizontalLayout{TState}"/>.
    /// </summary>
    public static class HorizontalLayout
    {
        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout<TResponse> Of<TResponse>(
            params IGuiComponent<TResponse>[] components
        )
        {
            return new(components.ToImmutableArray());
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout<TResponse> Of<TResponse>(
            IEnumerable<IGuiComponent<TResponse>> components
        )
        {
            return new(components.ToImmutableArray());
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout<Unit> Build(
            Action<ILayoutBuilder<Unit, HorizontalLayout<Unit>>> addComponents
        )
        {
            return HorizontalLayout.Build<Unit>(addComponents);
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        /// <returns>The horizontal layout.</returns>
        public static HorizontalLayout<TResponse> Build<TResponse>(
            Action<ILayoutBuilder<TResponse, HorizontalLayout<TResponse>>> addComponents
        )
        {
            var builder = new Builder<TResponse>();
            addComponents(builder);
            return builder.Build();
        }

        /// <summary>
        /// A horizontal layout builder.
        /// </summary>
        /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
        private class Builder<TResponse> : ILayoutBuilder<TResponse, HorizontalLayout<TResponse>>
        {
            private readonly List<IGuiComponent<TResponse>> components;

            /// <summary>
            /// Creates a new horizontal layout builder.
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
            /// <returns>The horizontal layout.</returns>
            public HorizontalLayout<TResponse> Build()
            {
                return new(this.components.ToImmutableArray());
            }
        }
    }

    /// <summary>
    /// A horizontal layout container. Components are rendered horizontally along a single row.
    /// </summary>
    /// <param name="Components">The components in this layout.</param>
    /// <typeparam name="TResponse">The type of the inner component's response.</typeparam>
    public record HorizontalLayout<TResponse>
        (ImmutableArray<IGuiComponent<TResponse>> Components) : IGuiComponent<
            IEnumerable<HorizontalLayout<TResponse>.ResponseItem>>
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
        public IEnumerable<ResponseItem> Handle(GuiEvent e, Rectangle bounds)
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
            var responses = new List<ResponseItem>(this.Components.Length);
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
                var response = sizedComponent.Component.Handle(
                    e,
                    new(bounds.X, bounds.Y, width, height)
                );
                responses.Add(new(sizedComponent.Component, response));

                // Update remaining area
                bounds = new(bounds.X + width, bounds.Y, Math.Max(0, bounds.Width - width), height);
            }

            return responses;
        }

        /// <summary>
        /// The type of response of a <see cref="HorizontalLayout{TResponse}"/>.
        /// </summary>
        /// <param name="Component">The component that responded.</param>
        /// <param name="Response">The component's response.</param>
        public record ResponseItem(IGuiComponent<TResponse> Component, TResponse Response);

        private class SizedComponent
        {
            public IGuiComponent<TResponse> Component { get; }
            public GuiConstraints Constraints { get; }
            public float AdditionalWidth { get; set; }

            public float MinWidth => this.Constraints.MinSize.Width;

            public float? RemainingWidth => this.Constraints.MaxSize.Width switch
            {
                null => null,
                { } maxWidth => maxWidth - this.Constraints.MinSize.Width - this.AdditionalWidth
            };

            public SizedComponent(IGuiComponent<TResponse> component, GuiConstraints constraints)
            {
                this.Component = component;
                this.Constraints = constraints;
                this.AdditionalWidth = 0;
            }
        }

        /// <summary>
        /// The state of a <see cref="HorizontalLayout{TState}"/> component.
        /// </summary>
        public class State
        {
            internal Rectangle Bounds { get; }

            internal ImmutableDictionary<IGuiComponent<TResponse>, TResponse> InnerStates
            {
                get;
            }

            internal State(
                Rectangle bounds,
                ImmutableDictionary<IGuiComponent<TResponse>, TResponse> innerStates
            )
            {
                this.Bounds = bounds;
                this.InnerStates = innerStates;
            }
        }
    }
}

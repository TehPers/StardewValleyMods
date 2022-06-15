using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// Utility methods for <see cref="VerticalLayout{TState}"/>.
    /// </summary>
    public static class VerticalLayout
    {
        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public static VerticalLayout<TState> Of<TState>(params IGuiComponent<TState>[] components)
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public static VerticalLayout<TState> Of<TState>(
            IEnumerable<IGuiComponent<TState>> components
        )
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The vertical layout.</returns>
        public static VerticalLayout<WrappedComponent.State> Build(Action<Builder> addComponents)
        {
            var builder = new Builder();
            addComponents(builder);
            return builder.Build();
        }

        /// <summary>
        /// A vertical layout builder.
        /// </summary>
        public class Builder : ILayoutBuilder<WrappedComponent.State,
            VerticalLayout<WrappedComponent.State>>
        {
            private readonly List<IGuiComponent<WrappedComponent.State>> components;

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
            public void Add(IGuiComponent<WrappedComponent.State> component)
            {
                this.components.Add(component);
            }

            /// <summary>
            /// Builds the layout from this builder.
            /// </summary>
            /// <returns>The vertical layout.</returns>
            public VerticalLayout<WrappedComponent.State> Build()
            {
                return Of(this.components);
            }
        }
    }

    /// <summary>
    /// A vertical layout container. Components are rendered vertically along a single column. To
    /// create a layout with different types of components, see <see cref="WrappedComponent"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the inner components' states.</typeparam>
    public class VerticalLayout<TState> : IGuiComponent<VerticalLayout<TState>.State>
    {
        /// <summary>The components in this layout.</summary>
        public ImmutableList<IGuiComponent<TState>> Components { get; init; }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="components">The components in this layout.</param>
        public VerticalLayout(ImmutableList<IGuiComponent<TState>> components)
        {
            this.Components = components ?? throw new ArgumentNullException(nameof(components));
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Components.Aggregate(
                new GuiConstraints { MaxSize = new(null, 0) },
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

        private IEnumerable<(IGuiComponent<TState> Component, Rectangle Bounds)> CalculateLayouts(
            Rectangle bounds
        )
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
                yield return (sizedComponent.Component, new(bounds.X, bounds.Y, width, height));

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
        public State Initialize(Rectangle bounds)
        {
            return new(
                bounds,
                this.CalculateLayouts(bounds)
                    .ToImmutableDictionary(
                        item => item.Component,
                        item => item.Component.Initialize(item.Bounds)
                    )
            );
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return new(
                bounds,
                this.CalculateLayouts(bounds)
                    .ToImmutableDictionary(
                        item => item.Component,
                        item => state.InnerStates.TryGetValue(item.Component, out var s)
                            ? item.Component.Reposition(s, item.Bounds)
                            : item.Component.Initialize(item.Bounds)
                    )
            );
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
            // Draw each child
            foreach (var component in this.Components)
            {
                // Ignore uninitialized components
                if (!state.InnerStates.TryGetValue(component, out var innerState))
                {
                    continue;
                }

                component.Draw(batch, innerState);
            }
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            var changed = false;

            // Make sure each child is initialized
            var needsReposition =
                this.Components.Any(component => !state.InnerStates.ContainsKey(component));
            if (needsReposition)
            {
                state = this.Reposition(state, state.Bounds);
                changed = true;
            }

            // Update all children
            var builder = state.InnerStates.ToBuilder();
            foreach (var component in this.Components)
            {
                if (!component.Update(e, state.InnerStates[component], out var nextInnerState))
                {
                    continue;
                }

                changed = true;
                builder[component] = nextInnerState;
            }

            // Check if any children changed
            if (changed)
            {
                nextState = new(state.Bounds, builder.ToImmutable());
                return true;
            }

            nextState = default;
            return false;
        }

        private class SizedComponent
        {
            public IGuiComponent<TState> Component { get; }
            public GuiConstraints Constraints { get; }
            public float AdditionalHeight { get; set; }

            public float MinHeight => this.Constraints.MinSize.Height;

            public float? RemainingHeight => this.Constraints.MaxSize.Height switch
            {
                null => null,
                { } maxHeight => maxHeight - this.Constraints.MinSize.Height - this.AdditionalHeight
            };

            public SizedComponent(IGuiComponent<TState> component, GuiConstraints constraints)
            {
                this.Component = component;
                this.Constraints = constraints;
                this.AdditionalHeight = 0;
            }
        }

        /// <summary>
        /// The state of a <see cref="VerticalLayout{TState}"/> component.
        /// </summary>
        public class State
        {
            internal Rectangle Bounds { get; }

            internal ImmutableDictionary<IGuiComponent<TState>, TState> InnerStates
            {
                get;
            }

            internal State(
                Rectangle bounds,
                ImmutableDictionary<IGuiComponent<TState>, TState> innerStates
            )
            {
                this.Bounds = bounds;
                this.InnerStates = innerStates;
            }
        }
    }
}

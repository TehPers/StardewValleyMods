using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TehPers.Core.Api.Gui
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
        public static HorizontalLayout<TState> Of<TState>(params IGuiComponent<TState>[] components)
        {
            return new(components.ToImmutableList());
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in the layout.</param>
        public static HorizontalLayout<TState> Of<TState>(
            IEnumerable<IGuiComponent<TState>> components
        )
        {
            return new(components.ToImmutableList());
        }
    }

    /// <summary>
    /// A horizontal layout container. Components are rendered horizontally along a single row. To
    /// create a layout with different types of components, see <see cref="WrappedComponent"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the inner components' states.</typeparam>
    public class HorizontalLayout<TState> : IGuiComponent<HorizontalLayout<TState>.State>
    {
        /// <summary>The components in this layout.</summary>
        public ImmutableList<IGuiComponent<TState>> Components { get; init; }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="components">The components in this layout.</param>
        public HorizontalLayout(ImmutableList<IGuiComponent<TState>> components)
        {
            this.Components = components ?? throw new ArgumentNullException(nameof(components));
        }

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

        private IEnumerable<(IGuiComponent<TState> Component, Rectangle Bounds)> CalculateLayouts(
            Rectangle bounds
        )
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
                yield return (sizedComponent.Component, new(bounds.X, bounds.Y, width, height));

                // Update remaining area
                bounds = new(bounds.X + width, bounds.Y, Math.Max(0, bounds.Width - width), height);
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
            public float AdditionalWidth { get; set; }

            public float MinWidth => this.Constraints.MinSize.Width;

            public float? RemainingWidth => this.Constraints.MaxSize.Width switch
            {
                null => null,
                { } maxWidth => maxWidth - this.Constraints.MinSize.Width - this.AdditionalWidth
            };

            public SizedComponent(IGuiComponent<TState> component, GuiConstraints constraints)
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

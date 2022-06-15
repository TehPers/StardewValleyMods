using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A non-generic wrapper around a GUI component.
    /// </summary>
    public class WrappedComponent : IGuiComponent<WrappedComponent.State>
    {
        /// <summary>
        /// The wrapped component.
        /// </summary>
        public object Component { get; }

        private readonly Func<GuiConstraints> getConstraints;
        private readonly Func<Rectangle, State> initialize;

        private WrappedComponent(
            object component,
            Func<GuiConstraints> getConstraints,
            Func<Rectangle, State> initialize
        )
        {
            this.Component = component;
            this.getConstraints = getConstraints;
            this.initialize = initialize;
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.getConstraints();
        }

        /// <inheritdoc />
        public State Initialize(Rectangle bounds)
        {
            return this.initialize(bounds);
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return state.Reposition(bounds);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
            state.Draw(batch);
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            if (state.Update(e) is { } newState)
            {
                nextState = newState;
                return true;
            }

            nextState = default;
            return false;
        }

        /// <summary>
        /// Creates a <see cref="WrappedComponent"/> from a GUI component.
        /// </summary>
        /// <typeparam name="TState">The type of the inner component's state.</typeparam>
        /// <param name="component">The inner component.</param>
        /// <returns>The wrapped component.</returns>
        public static WrappedComponent Of<TState>(IGuiComponent<TState> component)
        {
            _ = component ?? throw new ArgumentNullException(nameof(component));

            return new(
                component,
                component.GetConstraints,
                bounds => State.Of(component, component.Initialize(bounds))
            );
        }
        
        /// <summary>
        /// A wrapper around a GUI component's state.
        /// </summary>
        public class State
        {
            internal Func<Rectangle, State> Reposition { get; }
            internal Action<SpriteBatch> Draw { get; }
            internal Func<GuiEvent, State?> Update { get; }

            private State(
                Func<Rectangle, State> reposition,
                Action<SpriteBatch> draw,
                Func<GuiEvent, State?> update
            )
            {
                this.Reposition = reposition;
                this.Draw = draw;
                this.Update = update;
            }

            internal static State Of<TState>(IGuiComponent<TState> component, TState state)
            {
                // The generic types are captured and hidden by the callbacks
                return new(
                    bounds => State.Of(component, component.Reposition(state, bounds)),
                    (batch) => component.Draw(batch, state),
                    e => component.Update(e, state, out var nextState)
                        ? State.Of(component, nextState)
                        : default
                );
            }
        }
    }
}

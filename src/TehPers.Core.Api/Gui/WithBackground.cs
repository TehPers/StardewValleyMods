using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Draws a component with a background.
    /// </summary>
    /// <typeparam name="TFgState">The type of the foreground component's state.</typeparam>
    /// <typeparam name="TBgState">The type of the background component's state.</typeparam>
    public class
        WithBackground<TFgState, TBgState> : IGuiComponent<WithBackground<TFgState, TBgState>.State>
    {
        /// <summary>The inner component.</summary>
        public IGuiComponent<TFgState> Foreground { get; init; }

        /// <summary>The background component.</summary>
        public IGuiComponent<TBgState> Background { get; init; }

        /// <summary>
        /// Draws a component with a background.
        /// </summary>
        /// <param name="foreground">The inner component.</param>
        /// <param name="background">The background component.</param>
        public WithBackground(
            IGuiComponent<TFgState> foreground,
            IGuiComponent<TBgState> background
        )
        {
            this.Foreground = foreground;
            this.Background = background;
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var fgConstraints = this.Foreground.GetConstraints();
            var bgConstraints = this.Background.GetConstraints();
            return new()
            {
                MinSize = new(
                    Math.Max(bgConstraints.MinSize.Width, fgConstraints.MinSize.Width),
                    Math.Max(bgConstraints.MinSize.Height, fgConstraints.MinSize.Height)
                ),
                MaxSize = new(
                    (bgConstraints.MaxSize.Width, fgConstraints.MaxSize.Width) switch
                    {
                        (null, var w) => w,
                        (var w, null) => w,
                        ({ } w1, { } w2) => Math.Min(w1, w2),
                    },
                    (bgConstraints.MaxSize.Height, fgConstraints.MaxSize.Height) switch
                    {
                        (null, var h) => h,
                        (var h, null) => h,
                        ({ } h1, { } h2) => Math.Min(h1, h2),
                    }
                ),
                AllowBuffer = bgConstraints.AllowBuffer && fgConstraints.AllowBuffer,
            };
        }

        /// <inheritdoc />
        public State Initialize(Rectangle bounds)
        {
            return new(this.Foreground.Initialize(bounds), this.Background.Initialize(bounds));
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return new(
                this.Foreground.Reposition(state.FgState, bounds),
                this.Background.Reposition(state.BgState, bounds)
            );
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            var changed = false;
            nextState = state;

            // Update foreground state
            if (this.Foreground.Update(e, state.FgState, out var nextFgState))
            {
                changed = true;
                nextState = nextState with {FgState = nextFgState};
            }

            // Update background state
            if (this.Background.Update(e, state.BgState, out var nextBgState))
            {
                changed = true;
                nextState = nextState with {BgState = nextBgState};
            }

            return changed;
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
            this.Background.Draw(batch, state.BgState);
            this.Foreground.Draw(batch, state.FgState);
        }

        /// <summary>
        /// The state of a <see cref="WithBackground{TFgState,TBgState}"/> component.
        /// </summary>
        /// <param name="FgState">The foreground state.</param>
        /// <param name="BgState">The background state.</param>
        public record State(TFgState FgState, TBgState BgState);
    }
}

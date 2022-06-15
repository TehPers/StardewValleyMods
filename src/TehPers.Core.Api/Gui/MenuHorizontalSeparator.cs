using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A horizontal separator in a menu.
    /// </summary>
    public class MenuHorizontalSeparator : IGuiComponent<MenuHorizontalSeparator.State>
    {
        private readonly WrappedComponent inner;

        /// <summary>
        /// Creates a new horizontal menu separator.
        /// </summary>
        public MenuHorizontalSeparator()
        {
            this.inner = HorizontalLayout.Of(
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(0, 64, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(null, 1),
                        SourceRectangle = new(128, 64, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(192, 64, 64, 64),
                    }
                )
                .Wrapped();
        }

        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.inner.GetConstraints();
        }

        /// <inheritdoc />
        public State Initialize(Rectangle bounds)
        {
            return new(this.inner.Initialize(bounds));
        }

        /// <inheritdoc />
        public State Reposition(State state, Rectangle bounds)
        {
            return new(this.inner.Reposition(state.Inner, bounds));
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, State state)
        {
            this.inner.Draw(batch, state.Inner);
        }

        /// <inheritdoc />
        public bool Update(GuiEvent e, State state, [NotNullWhen(true)] out State? nextState)
        {
            if (this.inner.Update(e, state.Inner, out var nextInnerState))
            {
                nextState = new(nextInnerState);
                return true;
            }

            nextState = null;
            return false;
        }


        /// <summary>
        /// The state for a <see cref="MenuHorizontalSeparator"/> component.
        /// </summary>
        public class State
        {
            internal WrappedComponent.State Inner { get; }

            internal State(WrappedComponent.State inner)
            {
                this.Inner = inner;
            }
        }
    }
}

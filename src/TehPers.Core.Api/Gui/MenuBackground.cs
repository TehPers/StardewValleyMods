using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Fills a space with an empty menu background and borders.
    /// </summary>
    public class MenuBackground : IGuiComponent<MenuBackground.State>
    {
        private readonly WrappedComponent inner;

        /// <summary>
        /// Creates a new menu background.
        /// </summary>
        public MenuBackground()
        {
            this.inner = VerticalLayout.Of(
                    HorizontalLayout.Of(
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = PartialGuiSize.One,
                            SourceRectangle = new(0, 0, 64, 64),
                        }.Wrapped(),
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = new(null, 1),
                            SourceRectangle = new(128, 0, 64, 64),
                        }.Wrapped(),
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = PartialGuiSize.One,
                            SourceRectangle = new(192, 0, 64, 64),
                        }.Wrapped()
                    ),
                    HorizontalLayout.Of(
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = new(1, null),
                            SourceRectangle = new(0, 128, 64, 64),
                        }.Wrapped(),
                        new EmptySpace().Wrapped(),
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = new(1, null),
                            SourceRectangle = new(192, 128, 64, 64),
                        }.Wrapped()
                    ),
                    HorizontalLayout.Of(
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = PartialGuiSize.One,
                            SourceRectangle = new(0, 192, 64, 64),
                        }.Wrapped(),
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = new(null, 1),
                            SourceRectangle = new(128, 192, 64, 64),
                        }.Wrapped(),
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = PartialGuiSize.One,
                            SourceRectangle = new(192, 192, 64, 64),
                        }.Wrapped()
                    )
                )
                .WithBackground(
                    new StretchedTexture(Game1.menuTexture) {SourceRectangle = new(64, 128, 64, 64)}
                        .WithPadding(32, 32)
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
        /// The state for a <see cref="MenuBackground"/> component.
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A vertical separator in a menu.
    /// </summary>
    public class MenuVerticalSeparator : IGuiComponent<MenuVerticalSeparator.State>
    {
        private readonly WrappedComponent inner;

        /// <summary>
        /// A vertical separator in a menu.
        /// </summary>
        /// <param name="topConnector">The end T-connector to put at the top of this separator.</param>
        /// <param name="bottomConnector">The end T-connector to put at the bottom of this separator.</param>
        public MenuVerticalSeparator(
            MenuSeparatorConnector topConnector,
            MenuSeparatorConnector bottomConnector
        )
        {
            this.inner = VerticalLayout.Of(
                    Enumerable.Empty<StretchedTexture>()
                        .Concat(
                            topConnector switch
                            {
                                MenuSeparatorConnector.PinMenuBorder => new StretchedTexture[]
                                {
                                    new(Game1.menuTexture)
                                    {
                                        MinScale = GuiSize.One,
                                        MaxScale = PartialGuiSize.One,
                                        SourceRectangle = new(64, 0, 64, 64),
                                    }
                                }.AsEnumerable(),
                                MenuSeparatorConnector.MenuBorder => new StretchedTexture[]
                                {
                                    new(Game1.menuTexture)
                                    {
                                        MinScale = GuiSize.One,
                                        MaxScale = PartialGuiSize.One,
                                        SourceRectangle = new(0, 704, 64, 64),
                                    }
                                }.AsEnumerable(),
                                MenuSeparatorConnector.Separator => new StretchedTexture[]
                                {
                                    new(Game1.menuTexture)
                                    {
                                        MinScale = GuiSize.One,
                                        MaxScale = PartialGuiSize.One,
                                        SourceRectangle = new(192, 896, 64, 64),
                                    }
                                }.AsEnumerable(),
                                MenuSeparatorConnector.None => Enumerable.Empty<StretchedTexture>(),
                                _ => throw new ArgumentOutOfRangeException(
                                    nameof(topConnector),
                                    topConnector,
                                    null
                                )
                            }
                        )
                        .Append(
                            new(Game1.menuTexture)
                            {
                                MinScale = GuiSize.One,
                                MaxScale = new(1, null),
                                SourceRectangle = new(64, 64, 64, 64),
                            }
                        )
                        .Concat(
                            bottomConnector switch
                            {
                                MenuSeparatorConnector.PinMenuBorder => new[]
                                {
                                    new StretchedTexture(Game1.menuTexture)
                                    {
                                        MinScale = GuiSize.One,
                                        MaxScale = PartialGuiSize.One,
                                        SourceRectangle = new(64, 192, 64, 64),
                                    }
                                },
                                MenuSeparatorConnector.MenuBorder => new[]
                                {
                                    new StretchedTexture(Game1.menuTexture)
                                    {
                                        MinScale = GuiSize.One,
                                        MaxScale = PartialGuiSize.One,
                                        SourceRectangle = new(128, 960, 64, 64),
                                    }
                                },
                                MenuSeparatorConnector.Separator => new[]
                                {
                                    new StretchedTexture(Game1.menuTexture)
                                    {
                                        MinScale = GuiSize.One,
                                        MaxScale = PartialGuiSize.One,
                                        SourceRectangle = new(192, 576, 64, 64),
                                    }
                                },
                                MenuSeparatorConnector.None => Enumerable.Empty<StretchedTexture>(),
                                _ => throw new ArgumentOutOfRangeException(
                                    nameof(bottomConnector),
                                    bottomConnector,
                                    null
                                )
                            }
                        )
                )
                .Wrapped();
        }

        /// <summary>
        /// Creates a vertical separator with matching end T-connectors.
        /// </summary>
        /// <param name="connector">The T-connectors to add to the ends of the separator.</param>
        public MenuVerticalSeparator(MenuSeparatorConnector connector)
            : this(connector, connector)
        {
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

            nextState = default;
            return false;
        }

        /// <summary>
        /// The state for a <see cref="MenuVerticalSeparator"/> component.
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

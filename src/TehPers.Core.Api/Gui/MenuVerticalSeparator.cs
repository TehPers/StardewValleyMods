using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A vertical separator in a menu.
    /// </summary>
    /// <param name="TopConnector">The end T-connector to put at the top of this separator.</param>
    /// <param name="BottomConnector">The end T-connector to put at the bottom of this separator.</param>
    public record MenuVerticalSeparator(
        MenuSeparatorConnector TopConnector,
        MenuSeparatorConnector BottomConnector
    ) : BaseGuiComponent
    {
        private IGuiComponent Inner { get; init; } = new VerticalLayout(
            Enumerable.Empty<IGuiComponent>()
                .Concat(
                    TopConnector switch
                    {
                        MenuSeparatorConnector.PinMenuBorder => new[]
                        {
                            new StretchedTexture(Game1.menuTexture)
                            {
                                MinScale = GuiSize.One,
                                MaxScale = PartialGuiSize.One,
                                SourceRectangle = new(64, 0, 64, 64),
                            }
                        },
                        MenuSeparatorConnector.MenuBorder => new[]
                        {
                            new StretchedTexture(Game1.menuTexture)
                            {
                                MinScale = GuiSize.One,
                                MaxScale = PartialGuiSize.One,
                                SourceRectangle = new(0, 704, 64, 64),
                            }
                        },
                        MenuSeparatorConnector.Separator => new[]
                        {
                            new StretchedTexture(Game1.menuTexture)
                            {
                                MinScale = GuiSize.One,
                                MaxScale = PartialGuiSize.One,
                                SourceRectangle = new(192, 896, 64, 64),
                            }
                        },
                        MenuSeparatorConnector.None => Enumerable.Empty<IGuiComponent>(),
                        _ => throw new ArgumentOutOfRangeException(
                            nameof(TopConnector),
                            TopConnector,
                            null
                        )
                    }
                )
                .Append(
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(1, null),
                        SourceRectangle = new(64, 64, 64, 64),
                    }
                )
                .Concat(
                    BottomConnector switch
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
                        MenuSeparatorConnector.None => Enumerable.Empty<IGuiComponent>(),
                        _ => throw new ArgumentOutOfRangeException(
                            nameof(TopConnector),
                            TopConnector,
                            null
                        )
                    }
                )
                .ToImmutableList()
        );

        /// <inheritdoc />
        public override GuiConstraints Constraints => this.Inner.Constraints;

        /// <summary>
        /// Creates a vertical separator with matching end T-connectors.
        /// </summary>
        /// <param name="connector">The T-connectors to add to the ends of the separator.</param>
        public MenuVerticalSeparator(MenuSeparatorConnector connector)
            : this(connector, connector)
        {
        }

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.Inner.CalculateLayouts(bounds, layouts);
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            if (this.Inner.Update(e, componentBounds, out var newInner))
            {
                newComponent = this with {Inner = newInner};
                return true;
            }

            newComponent = default;
            return false;
        }
    }
}

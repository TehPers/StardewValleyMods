using StardewValley;
using System;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A vertical separator in a menu.
    /// </summary>
    public class MenuVerticalSeparator : WrapperComponent
    {
        /// <summary>
        /// A vertical separator in a menu.
        /// </summary>
        /// <param name="topConnector">The end T-connector to put at the top of this separator.</param>
        /// <param name="bottomConnector">The end T-connector to put at the bottom of this separator.</param>
        public MenuVerticalSeparator(
            MenuSeparatorConnector topConnector,
            MenuSeparatorConnector bottomConnector
        )
            : base(MenuVerticalSeparator.CreateInner(topConnector, bottomConnector))
        {
        }

        /// <summary>
        /// Creates a vertical separator with matching end T-connectors.
        /// </summary>
        /// <param name="connector">The T-connectors to add to the ends of the separator.</param>
        public MenuVerticalSeparator(MenuSeparatorConnector connector)
            : this(connector, connector)
        {
        }

        private static IGuiComponent CreateInner(
            MenuSeparatorConnector topConnector,
            MenuSeparatorConnector bottomConnector
        )
        {
            return VerticalLayout.Build(
                builder =>
                {
                    // Top connector
                    switch (topConnector)
                    {
                        case MenuSeparatorConnector.PinMenuBorder:
                            builder.Add(
                                new StretchedTexture(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(64, 0, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.MenuBorder:
                            builder.Add(
                                new StretchedTexture(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(0, 704, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.Separator:
                            builder.Add(
                                new StretchedTexture(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(192, 896, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(topConnector),
                                topConnector,
                                null
                            );
                    }

                    // Body
                    builder.Add(
                        new StretchedTexture(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = new(1, null),
                            SourceRectangle = new(64, 64, 64, 64),
                        }
                    );

                    // Bottom connector
                    switch (bottomConnector)
                    {
                        case MenuSeparatorConnector.PinMenuBorder:
                            builder.Add(
                                new StretchedTexture(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(64, 192, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.MenuBorder:
                            builder.Add(
                                new StretchedTexture(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(128, 960, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.Separator:
                            builder.Add(
                                new StretchedTexture(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(192, 576, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(bottomConnector),
                                bottomConnector,
                                null
                            );
                    }
                }
            );
        }
    }
}

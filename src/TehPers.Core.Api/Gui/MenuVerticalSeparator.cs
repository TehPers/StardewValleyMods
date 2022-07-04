using StardewValley;
using System;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A vertical separator in a menu.
    /// </summary>
    internal record MenuVerticalSeparator() : WrapperComponent
    {
        public override IGuiComponent Inner => this.CreateInner();

        /// <summary>
        /// The end T-connector to put at the top of this separator.
        /// </summary>
        public MenuSeparatorConnector TopConnector { get; init; } =
            MenuSeparatorConnector.MenuBorder;

        /// <summary>
        /// The end T-connector to put at the bottom of this separator.
        /// </summary>
        public MenuSeparatorConnector BottomConnector { get; init; } =
            MenuSeparatorConnector.MenuBorder;

        /// <summary>
        /// Creates a vertical separator with matching end T-connectors.
        /// </summary>
        /// <param name="connector">The T-connectors to add to the ends of the separator.</param>
        public MenuVerticalSeparator(MenuSeparatorConnector connector)
            : this()
        {
            this.TopConnector = connector;
            this.BottomConnector = connector;
        }

        private IGuiComponent CreateInner()
        {
            return VerticalLayoutComponent.BuildAligned(
                builder =>
                {
                    // Top connector
                    switch (this.TopConnector)
                    {
                        case MenuSeparatorConnector.PinMenuBorder:
                            builder.Add(
                                new TextureComponent(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(64, 0, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.MenuBorder:
                            builder.Add(
                                new TextureComponent(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(0, 704, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.Separator:
                            builder.Add(
                                new TextureComponent(Game1.menuTexture)
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
                                nameof(this.TopConnector),
                                this.TopConnector,
                                null
                            );
                    }

                    // Body
                    builder.Add(
                        new TextureComponent(Game1.menuTexture)
                        {
                            MinScale = GuiSize.One,
                            MaxScale = new(1, null),
                            SourceRectangle = new(64, 64, 64, 64),
                        }
                    );

                    // Bottom connector
                    switch (this.BottomConnector)
                    {
                        case MenuSeparatorConnector.PinMenuBorder:
                            builder.Add(
                                new TextureComponent(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(64, 192, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.MenuBorder:
                            builder.Add(
                                new TextureComponent(Game1.menuTexture)
                                {
                                    MinScale = GuiSize.One,
                                    MaxScale = PartialGuiSize.One,
                                    SourceRectangle = new(128, 960, 64, 64),
                                }
                            );
                            break;
                        case MenuSeparatorConnector.Separator:
                            builder.Add(
                                new TextureComponent(Game1.menuTexture)
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
                                nameof(MenuVerticalSeparator.BottomConnector),
                                this.BottomConnector,
                                null
                            );
                    }
                }
            );
        }
    }
}

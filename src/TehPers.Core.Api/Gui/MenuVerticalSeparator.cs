using StardewValley;
using System;
using TehPers.Core.Api.Gui.Components;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A vertical separator in a menu.
    /// </summary>
    internal record MenuVerticalSeparator() : ComponentWrapper
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
            return GuiComponent.Vertical(
                builder =>
                {
                    // Top connector
                    switch (this.TopConnector)
                    {
                        case MenuSeparatorConnector.PinMenuBorder:
                            GuiComponent.Texture(
                                    Game1.menuTexture,
                                    sourceRectangle: new(64, 0, 64, 64),
                                    minScale: GuiSize.One,
                                    maxScale: PartialGuiSize.One
                                )
                                .AddTo(builder);
                            break;
                        case MenuSeparatorConnector.MenuBorder:
                            GuiComponent.Texture(
                                    Game1.menuTexture,
                                    sourceRectangle: new(0, 704, 64, 64),
                                    minScale: GuiSize.One,
                                    maxScale: PartialGuiSize.One
                                )
                                .AddTo(builder);
                            break;
                        case MenuSeparatorConnector.Separator:
                            GuiComponent.Texture(
                                    Game1.menuTexture,
                                    sourceRectangle: new(192, 896, 64, 64),
                                    minScale: GuiSize.One,
                                    maxScale: PartialGuiSize.One
                                )
                                .AddTo(builder);
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
                    GuiComponent.Texture(
                            Game1.menuTexture,
                            sourceRectangle: new(64, 64, 64, 64),
                            minScale: GuiSize.One,
                            maxScale: new(1, null)
                        )
                        .AddTo(builder);

                    // Bottom connector
                    switch (this.BottomConnector)
                    {
                        case MenuSeparatorConnector.PinMenuBorder:
                            GuiComponent.Texture(
                                    Game1.menuTexture,
                                    sourceRectangle: new(64, 192, 64, 64),
                                    minScale: GuiSize.One,
                                    maxScale: PartialGuiSize.One
                                )
                                .AddTo(builder);
                            break;
                        case MenuSeparatorConnector.MenuBorder:
                            GuiComponent.Texture(
                                    Game1.menuTexture,
                                    sourceRectangle: new(128, 960, 64, 64),
                                    minScale: GuiSize.One,
                                    maxScale: PartialGuiSize.One
                                )
                                .AddTo(builder);
                            break;
                        case MenuSeparatorConnector.Separator:
                            GuiComponent.Texture(
                                    Game1.menuTexture,
                                    sourceRectangle: new(192, 576, 64, 64),
                                    minScale: GuiSize.One,
                                    maxScale: PartialGuiSize.One
                                )
                                .AddTo(builder);
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

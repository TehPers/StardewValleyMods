using StardewValley;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Fills a space with an empty menu background and borders.
    /// </summary>
    internal record MenuBackgroundComponent : WrapperComponent
    {
        public override IGuiComponent Inner => this.CreateInner();

        /// <summary>
        /// The layer depth to draw the component on.
        /// </summary>
        public float? LayerDepth { get; init; }
        
        private IGuiComponent CreateInner()
        {
            return VerticalLayoutComponent.Build(
                    builder =>
                    {
                        // Top row
                        builder.Add(
                            HorizontalLayoutComponent.Build(
                                builder =>
                                {
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: PartialGuiSize.One,
                                            sourceRectangle: new(0, 0, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: new(null, 1),
                                            sourceRectangle: new(128, 0, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: PartialGuiSize.One,
                                            sourceRectangle: new(192, 0, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                }
                            )
                        );

                        // Middle row
                        builder.Add(
                            HorizontalLayoutComponent.Build(
                                builder =>
                                {
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: new(1, null),
                                            sourceRectangle: new(0, 128, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                    builder.Add(GuiComponent.Empty());
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: new(1, null),
                                            sourceRectangle: new(192, 128, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                }
                            )
                        );

                        // Bottom row
                        builder.Add(
                            HorizontalLayoutComponent.Build(
                                builder =>
                                {
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: PartialGuiSize.One,
                                            sourceRectangle: new(0, 192, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: new(null, 1),
                                            sourceRectangle: new(128, 192, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                    builder.Add(
                                        GuiComponent.Texture(
                                            Game1.menuTexture,
                                            minScale: GuiSize.One,
                                            maxScale: PartialGuiSize.One,
                                            sourceRectangle: new(192, 192, 64, 64),
                                            layerDepth: this.LayerDepth
                                        )
                                    );
                                }
                            )
                        );
                    }
                )
                .WithBackground(
                    new TextureComponent(Game1.menuTexture) {SourceRectangle = new(64, 128, 64, 64)}
                        .WithPadding(32)
                );
        }
    }
}

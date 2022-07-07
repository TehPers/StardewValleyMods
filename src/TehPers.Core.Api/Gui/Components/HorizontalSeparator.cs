using StardewValley;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// A horizontal separator in a menu.
    /// </summary>
    internal record HorizontalSeparator : ComponentWrapper
    {
        public override IGuiComponent Inner => this.CreateInner();

        /// <summary>
        /// The layer depth to draw the component on.
        /// </summary>
        public float? LayerDepth { get; init; }

        private IGuiComponent CreateInner()
        {
            return HorizontalLayoutComponent.Of(
                GuiComponent.Texture(
                    Game1.menuTexture,
                    minScale: GuiSize.One,
                    maxScale: PartialGuiSize.One,
                    sourceRectangle: new(0, 64, 64, 64),
                    layerDepth: this.LayerDepth
                ),
                GuiComponent.Texture(
                    Game1.menuTexture,
                    minScale: GuiSize.One,
                    maxScale: new(null, 1),
                    sourceRectangle: new(128, 64, 64, 64),
                    layerDepth: this.LayerDepth
                ),
                GuiComponent.Texture(
                    Game1.menuTexture,
                    minScale: GuiSize.One,
                    maxScale: PartialGuiSize.One,
                    sourceRectangle: new(192, 64, 64, 64),
                    layerDepth: this.LayerDepth
                )
            );
        }
    }
}

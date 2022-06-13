using StardewValley;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// The root GUI state.
    /// </summary>
    public record RootGuiState
    {
        /// <summary>
        /// The component used to render this cursor.
        /// </summary>
        public IGuiComponent CursorComponent { get; init; } =
            new StretchedTexture(Game1.mouseCursors)
            {
                SourceRectangle = Game1.getSourceRectForStandardTileSheet(
                    Game1.mouseCursors,
                    Game1.options.gamepadControls ? 44 : 0,
                    16,
                    16
                ),
                MinScale = GuiSize.One,
            };
    }
}

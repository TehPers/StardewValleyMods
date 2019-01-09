using StardewValley;

namespace TehPers.CoreMod.Api.Drawing {
    internal class CraftablesTextureInfo : ITextureSourceInfo {
        /// <inheritdoc />
        public string TextureName { get; } = @"TileSheets\Craftables";

        /// <inheritdoc />
        public int GetIndexFromUV(int u, int v) {
            int tileX = u / 16;
            int tileY = v / 32;
            int widthInTiles = Game1.bigCraftableSpriteSheet.Width / 16;
            return tileY * widthInTiles + tileX;
        }

        public static ITextureSourceInfo Value { get; } = new CraftablesTextureInfo();
    }
}
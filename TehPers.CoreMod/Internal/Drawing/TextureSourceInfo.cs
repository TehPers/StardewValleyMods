using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Internal.Drawing {
    public class TextureSourceInfo : ITextureSourceInfo {
        
        /// <inheritdoc />
        public string TextureName { get; }

        /// <inheritdoc />
        public int TileWidth { get; }

        /// <inheritdoc />
        public int TileHeight { get; }

        public TextureSourceInfo(string textureName, int tileWidth, int tileHeight) {
            this.TextureName = textureName;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
        }
    }
}
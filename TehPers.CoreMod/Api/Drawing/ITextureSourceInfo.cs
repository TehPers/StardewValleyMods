namespace TehPers.CoreMod.Api.Drawing {
    public interface ITextureSourceInfo {

        /// <summary>The name of the texture asset.</summary>
        string TextureName { get; }

        /// <summary>The width of each item in the asset.</summary>
        int TileWidth { get; }

        /// <summary>The height of each item in the asset.</summary>
        int TileHeight { get; }
    }
}
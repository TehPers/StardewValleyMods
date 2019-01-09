using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Drawing {
    public interface IDrawingApi {
        /// <summary>A texture containing a single white pixel.</summary>
        Texture2D WhitePixel { get; }

        /// <summary>Gets a texture helper for a particular resource that can be used to modify how the texture is drawn.</summary>
        /// <param name="gameAsset">The resource to get the texture helper for.</param>
        /// <returns>The texture helper for the particular resource.</returns>
        ITextureDrawingHelper GetTextureHelper(GameAssetLocation gameAsset);
    }
}
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal interface ITextureTrackerInternal {
        void AddHelper(GameAssetLocation textureLocation, TextureDrawingHelper helper);
    }
}
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal interface ITextureTrackerInternal {
        void AddHelper(AssetLocation textureLocation, TrackedTexture trackedTexture);
    }
}
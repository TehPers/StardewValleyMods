using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class TextureTracker : IAssetEditor, ITextureTrackerInternal {
        private readonly IApiHelper _apiHelper;
        private readonly Dictionary<AssetLocation, TrackedTexture> _trackedTextures = new Dictionary<AssetLocation, TrackedTexture>();

        public TextureTracker(IApiHelper apiHelper) {
            this._apiHelper = apiHelper;
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return true;
        }

        public void Edit<T>(IAssetData asset) {
            // Convert the asset name into a GameAssetLocation
            AssetLocation assetLocation = new AssetLocation(asset.AssetName, ContentSource.GameContent);

            // Update the tracked helper, if any
            if (asset.Data is Texture2D texture && this._trackedTextures.TryGetValue(assetLocation, out TrackedTexture trackedTexture)) {
                trackedTexture.CurrentTexture = texture;
                DrawingDelegator.AddTrackedTexture(texture, trackedTexture);
            }
        }

        public void AddHelper(AssetLocation textureLocation, TrackedTexture trackedTexture) {
            this._trackedTextures.Add(textureLocation, trackedTexture);
            DrawingDelegator.AddTrackedTexture(this.GetCurrentTexture(textureLocation), trackedTexture);
        }

        private Texture2D GetCurrentTexture(AssetLocation textureLocation) {
            return textureLocation.Source.Match<ContentSource, Texture2D>()
                .When(ContentSource.GameContent, () => Game1.content.Load<Texture2D>(textureLocation.Path))
                .When(ContentSource.ModFolder, () => this._apiHelper.Owner.Helper.Content.Load<Texture2D>(textureLocation.Path))
                .ElseThrow();
        }
    }
}

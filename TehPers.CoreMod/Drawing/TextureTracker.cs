using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class TextureTracker : IAssetEditor, ITextureTrackerInternal {
        private readonly DrawingApi _api;
        private readonly Dictionary<GameAssetLocation, TextureDrawingHelper> _trackedHelpers = new Dictionary<GameAssetLocation, TextureDrawingHelper>();

        public TextureTracker(DrawingApi api) {
            this._api = api;
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return true;
        }

        public void Edit<T>(IAssetData asset) {
            // Convert the asset name into a GameAssetLocation
            GameAssetLocation assetLocation = new GameAssetLocation(asset.AssetName);

            // Update the tracked helper, if any
            if (asset.Data is Texture2D texture && this._trackedHelpers.TryGetValue(assetLocation, out TextureDrawingHelper helper)) {
                DrawingDelegator.AddTextureHelper(texture, helper);
            }
        }

        public void AddHelper(GameAssetLocation textureLocation, TextureDrawingHelper helper) {
            this._trackedHelpers.Add(textureLocation, helper);
            DrawingDelegator.AddTextureHelper(this.GetCurrentTexture(textureLocation), helper);
        }

        private Texture2D GetCurrentTexture(GameAssetLocation textureLocation) {
            // TODO: Texture source from either content folder or mod folder
            // Texture2D texture = gameAsset.Source.Match<ContentSource, Texture2D>()
            //     .When(ContentSource.GameContent, () => Game1.content.Load<Texture2D>(gameAsset.Path))
            //     .When(ContentSource.ModFolder, () => this._coreApiHelper.Owner.Helper.Content.Load<Texture2D>(gameAsset.Path))
            //     .ElseThrow();

            return Game1.content.Load<Texture2D>(textureLocation.Path);
        }
    }
}

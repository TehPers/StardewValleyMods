using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace TehPers.CoreMod.Internal.Items {
    internal class TextureAssetTracker : IAssetEditor {
        private readonly IContentHelper _contentHelper;
        private readonly Dictionary<string, Texture2D> _trackedTextures = new Dictionary<string, Texture2D>();
        private readonly Dictionary<Texture2D, string> _trackedNames = new Dictionary<Texture2D, string>();

        public TextureAssetTracker(IContentHelper contentHelper) {
            this._contentHelper = contentHelper;
        }

        public bool TryGetTracked(string name, out Texture2D texture) {
            return this._trackedTextures.TryGetValue(this.NormalizeAssetName(name), out texture);
        }

        public bool TryGetTracked(Texture2D texture, out string name) {
            return this._trackedNames.TryGetValue(texture, out name);
        }

        public string NormalizeAssetName(string assetName) {
            return this._contentHelper.NormaliseAssetName(assetName);
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return typeof(Texture2D).IsAssignableFrom(typeof(T));
        }

        public void Edit<T>(IAssetData asset) {
            string name = this.NormalizeAssetName(asset.AssetName);
            Texture2D newTexture = asset.AsImage().Data;

            if (this.TryGetTracked(name, out Texture2D oldTexture)) {
                this._trackedNames.Remove(oldTexture);
                this._trackedTextures.Remove(name);
            }

            this._trackedNames.Add(newTexture, name);
            this._trackedTextures.Add(name, newTexture);
        }
    }
}
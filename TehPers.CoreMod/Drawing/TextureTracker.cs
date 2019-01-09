using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class TextureTracker : IAssetEditor {
        private readonly DrawingApi _api;

        public TextureTracker(DrawingApi api) {
            this._api = api;
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return true;
        }

        public void Edit<T>(IAssetData asset) {
            var assetLocation = new GameAssetLocation(asset.AssetName, asset.)
        }
    }
}

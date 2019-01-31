using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Integration {
    internal class JsonAssetsItemProvider : IItemProvider {
        private readonly ICoreApi _coreApi;
        private readonly IJsonAssetsApi _jsonAssetsApi;

        public JsonAssetsItemProvider(ICoreApi coreApi, IJsonAssetsApi jsonAssetsApi) {
            this._coreApi = coreApi;
            this._jsonAssetsApi = jsonAssetsApi;
        }

        public bool TryCreate(in ItemKey key, out Item item) {
            if (this._coreApi.Owner.Helper.ModRegistry.Get(key.OwnerId) is IModInfo ownerInfo && ownerInfo.Manifest.ContentPackFor?.UniqueID == "spacechase0.JsonAssets") {
                // Try to get it as an object
                int index = this._jsonAssetsApi.GetObjectId(key.LocalKey);
                if (index >= 0) {
                    item = new SObject(Vector2.Zero, index, 1);
                    return true;
                }

                // Try to get it as a big craftable
                index = this._jsonAssetsApi.GetBigCraftableId(key.LocalKey);
                if (index >= 0) {
                    item = new SObject(Vector2.Zero, index);
                    return true;
                }
            }

            item = default;
            return false;
        }

        public bool IsInstanceOf(in ItemKey key, Item item) {
            if (!(item is SObject obj && this._coreApi.Owner.Helper.ModRegistry.Get(key.OwnerId) is IModInfo ownerInfo && ownerInfo.Manifest.ContentPackFor.UniqueID == "spacechase0.JsonAssets")) {
                return false;
            }

            // Try to get it as an object
            int index = this._jsonAssetsApi.GetObjectId(key.LocalKey);
            if (index >= 0) {
                return !obj.bigCraftable.Value && obj.ParentSheetIndex == index;
            }

            // Try to get it as a big craftable
            index = this._jsonAssetsApi.GetBigCraftableId(key.LocalKey);
            if (index >= 0) {
                return obj.bigCraftable.Value && obj.ParentSheetIndex == index;
            }

            return false;
        }

        public void InvalidateAssets() { }
    }
}

using StardewValley;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Items.Recipes {
    internal class ModIngredient : IIngredient {
        private readonly IItemApi _itemApi;
        private readonly ItemKey _key;

        public ModIngredient(IItemApi itemApi, ItemKey key) {
            this._itemApi = itemApi;
            this._key = key;
        }

        public bool TryGetOne(out Item item) {
            return this._itemApi.TryCreate(this._key, out item);
        }

        public bool IsInstanceOf(Item item) {
            return this._itemApi.IsInstanceOf(this._key, item);
        }
    }
}
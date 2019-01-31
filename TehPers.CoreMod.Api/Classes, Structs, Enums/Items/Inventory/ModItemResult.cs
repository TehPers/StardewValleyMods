using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public class ModItemResult : IItemResult {
        private readonly IItemApi _itemApi;
        private readonly ItemKey _key;

        public int Quantity { get; }

        public ModItemResult(IItemApi itemApi, ItemKey key, int quantity = 1) {
            this._itemApi = itemApi;
            this._key = key;
            this.Quantity = quantity;
        }

        public bool TryCreateOne(out Item result) {
            if (!this._itemApi.TryCreate(this._key, out result)) {
                return false;
            }

            result.Stack = this.Quantity;
            return true;
        }
    }
}
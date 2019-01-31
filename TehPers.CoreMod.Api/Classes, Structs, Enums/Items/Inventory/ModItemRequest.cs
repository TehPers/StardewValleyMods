using System;
using System.Linq;
using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public class ModItemRequest : IItemRequest {
        private readonly IItemApi _itemApi;
        private readonly ItemKey _key;

        public int Quantity { get; }

        public ModItemRequest(IItemApi itemApi, ItemKey key, int quantity = 1) {
            this.Quantity = quantity;
            this._itemApi = itemApi;
            this._key = key;
        }

        public bool Matches(Item item) {
            return this._itemApi.IsInstanceOf(this._key, item);
        }
    }
}

using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public class ItemRequest : IItemRequest {
        private readonly Item _item;
        public int Quantity { get; }

        public ItemRequest(Item item, int quantity) {
            this.Quantity = quantity;
            this._item = item;
        }

        public bool Matches(Item item) {
            return item == this._item || item.canStackWith(this._item);
        }
    }
}
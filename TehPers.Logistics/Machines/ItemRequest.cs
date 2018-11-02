using StardewValley;

namespace TehPers.Logistics.Machines {
    public readonly struct ItemRequest {
        /// <summary>The source item object.</summary>
        public Item Item { get; }

        /// <summary>The amount of the source item.</summary>
        public int Quantity { get; }

        public ItemRequest(Item item) : this(item, item.Stack) { }
        public ItemRequest(Item item, int quantity) {
            this.Item = item;
            this.Quantity = quantity;
        }
    }
}
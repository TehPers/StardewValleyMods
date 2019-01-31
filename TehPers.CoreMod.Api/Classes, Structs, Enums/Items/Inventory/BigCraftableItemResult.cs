using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public class BigCraftableItemResult : IItemResult {
        private readonly int _index;

        public int Quantity { get; }

        public BigCraftableItemResult(int index, int quantity = 1) {
            this._index = index;
            this.Quantity = quantity;
        }

        public bool TryCreateOne(out Item result) {
            result = new SObject(Vector2.Zero, this._index);
            return true;
        }
    }
}
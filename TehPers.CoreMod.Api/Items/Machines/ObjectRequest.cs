using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Api.Items.Machines {
    public readonly struct ObjectRequest {
        /// <summary>The source item object.</summary>
        public SObject Item { get; }

        /// <summary>The amount of the source item.</summary>
        public int Quantity { get; }

        public ObjectRequest(SObject item) : this(item, item.Stack) { }
        public ObjectRequest(SObject item, int quantity) {
            this.Item = item;
            this.Quantity = quantity;
        }
    }
}
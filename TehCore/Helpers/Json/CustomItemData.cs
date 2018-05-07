using System;

namespace TehCore.Helpers.Json {
    public class CustomItemData {
        public InventorySource Source { get; set; }
        public int Slot { get; set; }
        public object Data { get; set; }
        public Type ItemType { get; set; }

        public CustomItemData(InventorySource source, int slot, object data, Type itemType) {
            this.Source = source;
            this.Slot = slot;
            this.Data = data;
            this.ItemType = itemType;
        }
    }
}
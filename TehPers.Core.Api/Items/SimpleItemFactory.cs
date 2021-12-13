using System;
using StardewValley;

namespace TehPers.Core.Api.Items
{
    public class SimpleItemFactory : IItemFactory
    {
        private readonly Func<Item> createItem;

        public string ItemType { get; }

        public SimpleItemFactory(string itemType, Func<Item> createItem)
        {
            this.ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
            this.createItem = createItem ?? throw new ArgumentNullException(nameof(createItem));
        }

        public Item Create()
        {
            return this.createItem();
        }
    }
}
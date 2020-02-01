using System;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.Drawing.Sprites;
using TehPers.Core.Api.Items;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items
{
    public class SObjectItemProvider : IItemProvider
    {
        private readonly IModHelper helper;
        private readonly SObjectKeyStore keyStore;

        public SObjectItemProvider(
            IModHelper helper,
            SObjectKeyStore keyStore)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
        }

        public void InvalidateAssets()
        {
            this.helper.Content.InvalidateCache("Data/ObjectInformation");
            this.helper.Content.InvalidateCache("Maps/springobjects");
        }

        public bool IsInstanceOf(NamespacedId id, Item item)
        {
            return item is SObject obj
                   && !obj.bigCraftable.Value
                   && this.keyStore.TryGetIndex(id, out var index)
                   && item.ParentSheetIndex == index;
        }

        public bool TryCreate(NamespacedId id, out Item item)
        {
            if (this.keyStore.TryGetIndex(id, out var index))
            {
                item = new SObject(index, 1);
                return true;
            }

            item = default;
            return false;
        }

        public bool TryGetSprite(NamespacedId id, out ISprite sprite)
        {
            throw new NotImplementedException();
        }
    }
}
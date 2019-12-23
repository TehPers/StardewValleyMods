using System.Linq;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.Drawing.Sprites;
using TehPers.Core.Api.Items;

namespace TehPers.Core.Items
{
    internal class GlobalItemProvider : IGlobalItemProvider
    {
        private readonly IItemProvider[] itemProviders;

        public GlobalItemProvider(IItemProvider[] itemProviders)
        {
            this.itemProviders = itemProviders;
        }

        public void InvalidateAssets()
        {
            foreach (var itemProvider in this.itemProviders)
            {
                itemProvider.InvalidateAssets();
            }
        }

        public bool IsInstanceOf(NamespacedId id, Item item)
        {
            return this.itemProviders.Any(itemProvider => itemProvider.IsInstanceOf(id, item));
        }

        public bool TryCreate(NamespacedId id, out Item item)
        {
            foreach (var itemProvider in this.itemProviders)
            {
                if (itemProvider.TryCreate(id, out item))
                {
                    return true;
                }
            }

            item = default;
            return false;
        }

        public bool TryGetSprite(NamespacedId id, out ISprite sprite)
        {
            foreach (var itemProvider in this.itemProviders)
            {
                if (itemProvider.TryGetSprite(id, out sprite))
                {
                    return true;
                }
            }

            sprite = default;
            return false;
        }
    }
}

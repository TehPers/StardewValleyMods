using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Items.ItemProviders;

namespace TehPers.CoreMod.Items {
    internal class ItemApi : IItemApi {
        private readonly IApiHelper _coreApiHelper;
        private readonly ItemDelegator2 _itemDelegator;

        public IDefaultItemProviders DefaultItemProviders { get; }

        public ItemApi(IApiHelper coreApiHelper, ItemDelegator2 itemDelegator) {
            this._coreApiHelper = coreApiHelper;
            this._itemDelegator = itemDelegator;
            this.DefaultItemProviders = new DefaultItemProviders(coreApiHelper, itemDelegator);
        }

        public bool TryCreate(string localKey, out Item item) {
            return this.TryCreate(new ItemKey(this._coreApiHelper.Owner, localKey), out item);
        }

        public bool TryCreate(ItemKey key, out Item item) {
            return this._itemDelegator.TryCreate(key, out item);
        }

        public void AddProvider(Func<IItemDelegator, IItemProvider> providerFactory) {
            this._itemDelegator.AddProvider(providerFactory);
            this._coreApiHelper.Log("Item provider registered", LogLevel.Trace);
        }

        public ISprite CreateSprite(Texture2D texture, Rectangle? sourceRectangle = null) {
            return this._itemDelegator.CustomItemSpriteSheet.Add(this._coreApiHelper, texture, sourceRectangle ?? texture.Bounds);
        }
    }
}
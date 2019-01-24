using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Items.ItemProviders {
    internal class ObjectProvider : IObjectProvider, IAssetEditor {
        private readonly IApiHelper _apiHelper;
        private readonly IItemDelegator _itemDelegator;
        private readonly Dictionary<ItemKey, IModObject> _objectManagers = new Dictionary<ItemKey, IModObject>();

        public ObjectProvider(IApiHelper apiHelper, IItemDelegator itemDelegator) {
            this._apiHelper = apiHelper;
            this._itemDelegator = itemDelegator;
        }

        public ItemKey Register(string localKey, IModObject objectManager) {
            // Create a new key
            ItemKey key = new ItemKey(this._apiHelper.Owner, localKey);

            // Call the other overload to register the key
            this.Register(key, objectManager);

            return key;
        }

        public void Register(ItemKey key, IModObject objectManager) {
            // Check if this key is already registered to an object manager
            if (this._objectManagers.ContainsKey(key)) {
                this._apiHelper.Log($"Attempted to register the same object twice: {key}", LogLevel.Warn);
                return;
            }

            // Try to register this key with the item delegator
            if (!this._itemDelegator.TryRegisterKey(key)) {
                throw new ArgumentException($"Key already registered: {key}", nameof(key));
            }

            // Track this key for later
            this._objectManagers.Add(key, objectManager);
        }

        public bool TryCreateItem(ItemKey key, out Item item) {
            // Try to get the index for the given key
            if (this._itemDelegator.TryGetIndex(key, out int index)) {
                item = new SObject(Vector2.Zero, index, 1);
                return true;
            }

            // Failed
            item = default;
            return false;
        }

        public bool IsInstanceOf(ItemKey key, Item item) {
            return item is SObject obj && !obj.bigCraftable.Value && this._itemDelegator.TryGetIndex(key, out int index) && item.ParentSheetIndex == index;
        }

        public void InvalidateAssets() {
            // Invalidate "Data/ObjectInformation" if necessary
            if (this._objectManagers.Any()) {
                this._apiHelper.Owner.Helper.Content.InvalidateCache("Data/ObjectInformation");
            }
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals("Data/ObjectInformation");
        }

        public void Edit<T>(IAssetData asset) {
            // This should only edit "Data/ObjectInformation"
            if (!asset.AssetNameEquals("Data/ObjectInformation")) {
                return;
            }

            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach ((ItemKey key, IModObject manager) in this._objectManagers) {
                if (this._itemDelegator.TryGetIndex(key, out int index)) {
                    data[index] = manager.GetRawObjectInformation();
                }
            }
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Drawing.Sprites;

namespace TehPers.CoreMod.Items {
    internal class ItemApi : IItemApi {
        private readonly IApiHelper _coreApiHelper;
        private readonly DynamicSpriteSheet _customItemSpriteSheet;

        public ItemApi(IApiHelper coreApiHelper, DynamicSpriteSheet customItemSpriteSheet) {
            this._coreApiHelper = coreApiHelper;
            this._customItemSpriteSheet = customItemSpriteSheet;
        }

        public ItemKey Register(string localKey, ISpriteSheet parentSheet, IModObject objectManager) {
            // Create the item key
            ItemKey key = new ItemKey(this._coreApiHelper.Owner, localKey);

            // Register the item
            if (!ItemDelegator.Register(key, parentSheet, objectManager)) {
                this._coreApiHelper.Log($"Attempted to register the same key twice: {key}");
            }

            // Return the key
            return key;
        }

        public bool TryGetObjectManager(string localKey, out IModObject manager) {
            return this.TryGetObjectManager(new ItemKey(this._coreApiHelper.Owner, localKey), out manager);
        }

        public bool TryGetObjectManager(ItemKey key, out IModObject manager) {
            if (ItemDelegator.TryGetInformation(key, out IObjectInformation info)) {
                manager = info.Manager;
                return true;
            }

            manager = default;
            return false;
        }

        public bool TryGetIndex(string localKey, out int index) {
            return this.TryGetIndex(new ItemKey(this._coreApiHelper.Owner, localKey), out index);
        }

        public bool TryGetIndex(ItemKey key, out int index) {
            if (ItemDelegator.TryGetInformation(key, out IObjectInformation info) && info.Index is int storedIndex) {
                index = storedIndex;
                return true;
            }

            index = default;
            return false;
        }

        public ISprite CreateSprite(Texture2D texture, Rectangle? sourceRectangle = null) {
            return this._customItemSpriteSheet.Add(this._coreApiHelper, texture, sourceRectangle ?? texture.Bounds);
        }
    }
}
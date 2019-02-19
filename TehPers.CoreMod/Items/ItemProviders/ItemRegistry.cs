using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;

namespace TehPers.CoreMod.Items.ItemProviders {
    internal abstract class ItemRegistry<TManager> : IItemRegistry<TManager> where TManager : IModItem {
        protected IApiHelper ApiHelper { get; }
        protected IItemDelegator ItemDelegator { get; }
        protected Dictionary<ItemKey, TManager> Managers { get; } = new Dictionary<ItemKey, TManager>();

        /// <inheritdoc />
        public IEnumerable<int> ReservedIndexes => Enumerable.Empty<int>();

        protected ItemRegistry(IApiHelper apiHelper, IItemDelegator itemDelegator) {
            this.ApiHelper = apiHelper;
            this.ItemDelegator = itemDelegator;
        }

        /// <inheritdoc />
        public ItemKey Register(string localKey, TManager manager) {
            // Create a new key
            ItemKey key = new ItemKey(this.ApiHelper.Owner, localKey);

            // Call the other overload to register the key
            this.Register(key, manager);

            return key;
        }

        /// <inheritdoc />
        public void Register(in ItemKey key, TManager manager) {
            // Try to register this key with the item delegator
            if (!this.ItemDelegator.TryRegisterKey(key)) {
                throw new ArgumentException($"Key already registered: {key}", nameof(key));
            }

            // Override its drawing
            this.ItemDelegator.OverrideSprite(key, this.GetSpriteSheet(key, manager), manager.OverrideDraw);

            // Track this key for later
            this.Managers.Add(key, manager);
        }

        /// <inheritdoc />
        public bool TryCreate(in ItemKey key, out Item item) {
            // Try to get the index for the given key
            if (this.ItemDelegator.TryGetIndex(key, out int index)) {
                item = this.CreateSingleItem(key, index);
                return true;
            }

            // None exists
            item = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetSprite(in ItemKey key, out ISprite sprite) {
            // Try to get the manager for the given key
            if (this.Managers.TryGetValue(key, out TManager manager)) {
                sprite = manager.Sprite;
                return true;
            }

            // Not found
            sprite = default;
            return false;
        }

        /// <inheritdoc />
        public abstract bool IsInstanceOf(in ItemKey key, Item item);

        /// <inheritdoc />
        public abstract void InvalidateAssets();

        /// <summary>Gets the sprite sheet the item with the given item key and manager is drawn from.</summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="manager">The manager for the item.</param>
        /// <returns>The sprite sheet that item is drawn from.</returns>
        protected abstract ISpriteSheet GetSpriteSheet(ItemKey key, TManager manager);

        /// <summary>Creates an instance of the item with the given item key and index. The item should have a stack size of 1.</summary>
        /// <param name="key">The item key for the item.</param>
        /// <param name="index">The index assigned to the item.</param>
        /// <returns>An <see cref="Item"/> with a stack size of 1 which is represented by the given item key.</returns>
        protected abstract Item CreateSingleItem(ItemKey key, int index);
    }
}
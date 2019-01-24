using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.ItemProviders;

namespace TehPers.CoreMod.Api.Items {
    public interface IItemApi {
        /// <summary>Default item provders. You can register items here.</summary>
        IDefaultItemProviders DefaultItemProviders { get; }
        
        /// <summary>Tries to create an instance of the specified item.</summary>
        /// <param name="localKey">The key for the item.</param>
        /// <param name="item">The created item, if successful, with a stack size of 1.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryCreate(string localKey, out Item item);

        /// <summary>Tries to create an instance of the specified item.</summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="item">The created item, if successful, with a stack size of 1.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryCreate(ItemKey key, out Item item);

        /// <summary>Adds a new item provider, which can be used to construct items from their keys. This is useful for custom content packs, for example.</summary>
        /// <param name="providerFactory">A method which creates a new item provider from the given item delegator.</param>
        void AddProvider(Func<IItemDelegator, IItemProvider> providerFactory);

        /// <summary>Creates an item sprite on a dynamically-generated sprite sheet to improve drawing performance. This will cause the sprite to be copied onto a sprite sheet built specifically for custom items.</summary>
        /// <param name="texture">The texture containing the item's sprite.</param>
        /// <param name="sourceRectangle">The source rectangle for the item's sprite.</param>
        /// <returns>A sprite object pointing to your item's sprite on the dynamically-created sprite sheet.</returns>
        ISprite CreateSprite(Texture2D texture, Rectangle? sourceRectangle = null);
    }
}
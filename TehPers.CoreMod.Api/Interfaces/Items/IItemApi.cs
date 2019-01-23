using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Conflux.Collections;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public interface IItemApi {
        /// <summary>Registers a new item with the game.</summary>
        /// <param name="localKey">The local key for this item, unique within your mod.</param>
        /// <param name="parentSheet">The sprite sheet the object would normally be drawn from. This determines the class of item it is. <seealso cref="IDrawingApi"/> has references to some commonly used sprite sheets, like <seealso cref="IDrawingApi.ObjectSpriteSheet"/>.</param>
        /// <param name="objectManager">The object's manager.</param>
        /// <returns>The key for the item once registered.</returns>
        ItemKey Register(string localKey, ISpriteSheet parentSheet, IModObject objectManager);

        /// <summary>Tries to get the manager for a registered item from its key.</summary>
        /// <param name="localKey">The local key for the item.</param>
        /// <param name="manager">The manager for the item, if found.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetObjectManager(string localKey, out IModObject manager);

        /// <summary>Tries to get the manager for a registered item from its key.</summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="manager">The manager for the item, if found.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetObjectManager(ItemKey key, out IModObject manager);

        /// <summary>Tries to get the index for a registered item from its key.</summary>
        /// <param name="localKey">The local key for the item.</param>
        /// <param name="index">The index of the item in its parent sheet, if found.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetIndex(string localKey, out int index);

        /// <summary>Tries to get the index for a registered item from its key.</summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="index">The index of the item in its parent sheet, if found.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetIndex(ItemKey key, out int index);

        /// <summary>Creates an item sprite on a dynamically-generated sprite sheet to improve drawing performance. This will cause the sprite to be copied onto a sprite sheet built specifically for custom items.</summary>
        /// <param name="texture">The texture containing the item's sprite.</param>
        /// <param name="sourceRectangle">The source rectangle for the item's sprite.</param>
        /// <returns>A sprite object pointing to your item's sprite on the dynamically-created sprite sheet.</returns>
        ISprite CreateSprite(Texture2D texture, Rectangle? sourceRectangle = null);
    }
}
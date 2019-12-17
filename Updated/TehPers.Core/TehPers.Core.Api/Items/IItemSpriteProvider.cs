using TehPers.Core.Api.Drawing.Sprites;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Sprite provider for items.
    /// </summary>
    public interface IItemSpriteProvider
    {
        /// <summary>
        /// Tries to get the sprite for a particular item.
        /// </summary>
        /// <param name="id">The item's key.</param>
        /// <param name="sprite">The sprite associated with the item.</param>
        /// <returns>True if the sprite was retrieved, false if this provider cannot provide a sprite for the given key.</returns>
        bool TryGetSprite(in NamespacedId id, out ISprite sprite);
    }
}
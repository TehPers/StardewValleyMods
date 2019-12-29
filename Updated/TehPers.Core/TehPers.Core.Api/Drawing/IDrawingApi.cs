using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Drawing.Sprites;

namespace TehPers.Core.Api.Drawing
{
    public interface IDrawingApi
    {
        /// <summary>
        /// Gets a texture containing a single white pixel.
        /// </summary>
        Texture2D WhitePixel { get; }

        /// <summary>
        /// Gets the sprite sheet containing all normal objects, linked to "Maps/springobjects".
        /// </summary>
        ISpriteSheet ObjectSpriteSheet { get; }

        /// <summary>
        /// Gets the sprite sheet containing all weapons, linked to "TileSheets/weapons".
        /// </summary>
        ISpriteSheet WeaponSpriteSheet { get; }

        /// <summary>
        /// Gets the sprite sheet containing all big craftables, linked to "TileSheets/Craftables".
        /// </summary>
        ISpriteSheet CraftableSpriteSheet { get; }

        /// <summary>
        /// Gets the sprite sheet containing all hats, linked to "Character/Farmer/hats".
        /// </summary>
        IHatSpriteSheet HatSpriteSheet { get; }

        /// <summary>
        /// Gets a texture helper for a particular resource that can be used to modify how the texture is drawn.
        /// </summary>
        /// <param name="asset">The resource to get the texture helper for.</param>
        /// <returns>The texture helper for the particular resource.</returns>
        ITrackedTexture GetTrackedTexture(AssetPath asset);

        /// <summary>
        /// Creates a simple sprite sheet.
        /// </summary>
        /// <param name="trackedTexture">The texture to create the sprite sheet from.</param>
        /// <param name="spriteWidth">The width of each sprite in the sprite sheet.</param>
        /// <param name="spriteHeight">The height of each sprite in the sprite sheet.</param>
        /// <returns>A new sprite sheet which can provide individual sprites in the texture.</returns>
        ISpriteSheet CreateSimpleSpriteSheet(ITrackedTexture trackedTexture, int spriteWidth, int spriteHeight);
    }
}
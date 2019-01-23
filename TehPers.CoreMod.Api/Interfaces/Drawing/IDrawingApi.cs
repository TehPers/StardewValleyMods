﻿using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Drawing {
    public interface IDrawingApi {
        /// <summary>A texture containing a single white pixel.</summary>
        Texture2D WhitePixel { get; }

        /// <summary>Gets a texture helper for a particular resource that can be used to modify how the texture is drawn.</summary>
        /// <param name="asset">The resource to get the texture helper for.</param>
        /// <returns>The texture helper for the particular resource.</returns>
        ITrackedTexture GetTrackedTexture(AssetLocation asset);
        
        /// <summary>Creates a simple sprite sheet.</summary>
        /// <param name="trackedTexture">The texture to create the sprite sheet from.</param>
        /// <param name="spriteWidth">The width of each sprite in the sprite sheet.</param>
        /// <param name="spriteHeight">The height of each sprite in the sprite sheet.</param>
        /// <returns>A new sprite sheet which can provide individual sprites in the texture.</returns>
        ISpriteSheet CreateSimpleSpriteSheet(ITrackedTexture trackedTexture, int spriteWidth, int spriteHeight);

        /// <summary>The sprite sheet containing all normal objects, linked to "Maps/springobjects".</summary>
        ISpriteSheet ObjectSpriteSheet { get; }

        /// <summary>The sprite sheet containing all big craftables, linked to "TileSheets/Craftables".</summary>
        ISpriteSheet CraftableSpriteSheet { get; }
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Information about how to render an <see cref="Item"/>.
    /// </summary>
    public class ItemSpriteInfo
    {
        /// <summary>
        /// The source <see cref="Texture2D"/> this item's sprite is rendered from.
        /// </summary>
        public Texture2D SourceTexture { get; }

        /// <summary>
        /// The <see cref="Rectangle"/> in the <see cref="SourceTexture"/> this item's sprite is located at.
        /// </summary>
        public Rectangle SourceRectangle { get; }

        /// <summary>
        /// The <see cref="Color"/> to tint this texture.
        /// </summary>
        public Color Tint { get; }

        public ItemSpriteInfo(Texture2D sourceTexture, Rectangle sourceRectangle)
            : this(sourceTexture, sourceRectangle, Color.White)
        {
        }

        public ItemSpriteInfo(Texture2D sourceTexture, Rectangle sourceRectangle, Color tint)
        {
            this.SourceTexture = sourceTexture ?? throw new ArgumentNullException(nameof(sourceTexture));
            this.SourceRectangle = sourceRectangle;
            this.Tint = tint;
        }
    }
}
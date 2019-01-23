using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Api.Drawing.Sprites {
    public interface ISprite : ITextureEvents {
        /// <summary>This sprite's index in its parent sheet.</summary>
        int Index { get; }

        /// <summary>The sprite sheet this sprite comes from.</summary>
        ISpriteSheet ParentSheet { get; }

        /// <summary>The source rectangle for this sprite from the parent sheet.</summary>
        Rectangle? SourceRectangle { get; }

        /// <summary>The width of this sprite.</summary>
        int Width { get; }

        /// <summary>The height of this sprite.</summary>
        int Height { get; }

        void Draw(SpriteBatch batch, Vector2 position, Color color);
        void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
        void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
        void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color);
        void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.SpriteMain.Patches
{
    public readonly struct TextureCacheResult
    {
        public Texture2D ScaledTexture { get; }

        public Rectangle SourceRectangle { get; }

        public float OriginScale { get; }

        public TextureCacheResult(Texture2D scaledTexture, Rectangle sourceRectangle, float originScale)
        {
            this.ScaledTexture = scaledTexture;
            this.SourceRectangle = sourceRectangle;
            this.OriginScale = originScale;
        }
    }
}
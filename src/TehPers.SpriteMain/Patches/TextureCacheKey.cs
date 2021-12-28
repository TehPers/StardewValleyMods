using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.SpriteMain.Patches
{
    internal readonly struct TextureCacheKey : IEquatable<TextureCacheKey>
    {
        public Texture2D Texture { get; }

        public Rectangle Source { get; }

        public TextureCacheKey(Texture2D texture, Rectangle source)
        {
            this.Texture = texture;
            this.Source = source;
        }

        public bool Equals(TextureCacheKey other)
        {
            return this.Texture.Equals(other.Texture) && this.Source.Equals(other.Source);
        }

        public override bool Equals(object? obj)
        {
            return obj is TextureCacheKey other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Texture, this.Source);
        }
    }
}
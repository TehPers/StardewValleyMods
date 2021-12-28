using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace TehPers.SpriteMain.Scalers
{
    internal abstract class GeneralScaler : IScaler
    {
        public abstract float Scale { get; }

        public Rectangle DrawScaled(Texture2D texture, Rectangle source, Texture2D destTexture)
        {
            // Verify that the texture formats match
            if (texture.Format != destTexture.Format)
            {
                throw new InvalidOperationException(
                    "Source and destination textures must have the same format."
                );
            }

            // Flip negative width/height
            if (source.Width < 0)
            {
                source = new(source.X + source.Width, source.Y, -source.Width, source.Height);
            }

            if (source.Height < 0)
            {
                source = new(source.X, source.Y + source.Height, source.Width, -source.Height);
            }

            // Constrain source rectangle
            var newX = Math.Clamp(source.X, 0, texture.Width);
            var newY = Math.Clamp(source.Y, 0, texture.Height);
            var newW = Math.Clamp(source.Width, 0, texture.Width - newX);
            var newH = Math.Clamp(source.Height, 0, texture.Height - newY);
            source = new(newX, newY, newW, newH);

            // Draw the scaled sprite to the destination
            return texture.Format switch
            {
                SurfaceFormat.Color => this.DrawScaled<Color>(texture, source, destTexture),
                SurfaceFormat.Bgr565 => this.DrawScaled<Bgr565>(texture, source, destTexture),
                SurfaceFormat.Bgra5551 => this.DrawScaled<Bgra5551>(texture, source, destTexture),
                SurfaceFormat.Bgra4444 => this.DrawScaled<Bgra4444>(texture, source, destTexture),
                SurfaceFormat.NormalizedByte2 => this.DrawScaled<NormalizedByte2>(
                    texture,
                    source,
                    destTexture
                ),
                SurfaceFormat.NormalizedByte4 => this.DrawScaled<NormalizedByte4>(
                    texture,
                    source,
                    destTexture
                ),
                SurfaceFormat.Rgba1010102 => this.DrawScaled<Rgba1010102>(
                    texture,
                    source,
                    destTexture
                ),
                SurfaceFormat.Rg32 => this.DrawScaled<Rg32>(texture, source, destTexture),
                SurfaceFormat.Rgba64 => this.DrawScaled<Rgba64>(texture, source, destTexture),
                SurfaceFormat.Alpha8 => this.DrawScaled<Alpha8>(texture, source, destTexture),
                SurfaceFormat.Single => this.DrawScaled<float>(texture, source, destTexture),
                SurfaceFormat.Vector2 => this.DrawScaled<Vector2>(texture, source, destTexture),
                SurfaceFormat.Vector4 => this.DrawScaled<Vector4>(texture, source, destTexture),
                SurfaceFormat.HalfSingle => this.DrawScaled<HalfSingle>(
                    texture,
                    source,
                    destTexture
                ),
                SurfaceFormat.HalfVector2 => this.DrawScaled<HalfVector2>(
                    texture,
                    source,
                    destTexture
                ),
                SurfaceFormat.HalfVector4 => this.DrawScaled<HalfVector4>(
                    texture,
                    source,
                    destTexture
                ),
                _ => throw new InvalidOperationException(
                    $"Unsupported texture format: {texture.Format}"
                )
            };
        }

        protected abstract Rectangle DrawScaled<T>(
            Texture2D texture,
            Rectangle source,
            Texture2D destTexture
        )
            where T : struct;
    }
}
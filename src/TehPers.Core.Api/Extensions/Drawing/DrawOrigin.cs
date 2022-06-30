using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    public record DrawOrigin(Vector2 SourceSize, Vector2 OriginInSource) : IDrawOrigin
    {
        public Vector2 GetTranslation(Vector2 size)
        {
            var scale = size / this.SourceSize;
            var scaledOrigin = this.OriginInSource * scale;
            return -scaledOrigin;
        }
    }
}
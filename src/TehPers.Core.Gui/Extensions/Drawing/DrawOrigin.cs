using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Positions the drawn sprite relative to a known size and origin for that size.
/// </summary>
/// <param name="SourceSize">The known size.</param>
/// <param name="OriginInSource">The position within the known size that should be the origin.</param>
public record DrawOrigin(Vector2 SourceSize, Vector2 OriginInSource) : IDrawOrigin
{
    /// <inheritdoc />
    public Vector2 GetTranslation(Vector2 size)
    {
        var scale = size / this.SourceSize;
        var scaledOrigin = this.OriginInSource * scale;
        return -scaledOrigin;
    }
}

using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Properties about how furniture is drawn in menus.
/// </summary>
/// <param name="SourceSize">The size of the furniture.</param>
/// <param name="ScaleSize">The scaling used for this furniture.</param>
public record FurnitureDrawingProperties
    (Vector2 SourceSize, float ScaleSize) : IDrawingProperties
{
    /// <inheritdoc />
    public Vector2 Offset(float scaleSize)
    {
        return new(32f, 32f);
    }

    /// <inheritdoc />
    public Vector2 Origin(float scaleSize)
    {
        return this.SourceSize / 2f;
    }

    /// <inheritdoc />
    public float RealScale(float scaleSize)
    {
        return this.ScaleSize * scaleSize;
    }
}

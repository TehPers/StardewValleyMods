using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Properties about how big craftables are drawn in menus.
/// </summary>
public record BigCraftableDrawingProperties : IDrawingProperties
{
    /// <inheritdoc />
    public Vector2 SourceSize => new(16f, 32f);

    /// <inheritdoc />
    public Vector2 Offset(float scaleSize)
    {
        return new(32f, 32f);
    }

    /// <inheritdoc />
    public Vector2 Origin(float scaleSize)
    {
        return new(8f, 16f);
    }

    /// <inheritdoc />
    public float RealScale(float scaleSize)
    {
        return 4f * (scaleSize < 0.2f ? scaleSize : scaleSize / 2f);
    }
}

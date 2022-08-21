using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Properties about how tools are drawn in menus.
/// </summary>
public record ToolDrawingProperties : IDrawingProperties
{
    /// <inheritdoc />
    public Vector2 SourceSize => new(16f, 16f);

    /// <inheritdoc />
    public Vector2 Offset(float scaleSize)
    {
        return new(32f, 32f);
    }

    /// <inheritdoc />
    public Vector2 Origin(float scaleSize)
    {
        return new(8f, 8f);
    }

    /// <inheritdoc />
    public float RealScale(float scaleSize)
    {
        return 4f * scaleSize;
    }
}

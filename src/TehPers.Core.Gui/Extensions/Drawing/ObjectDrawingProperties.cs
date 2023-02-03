﻿using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Properties about how normal objects are drawn in menus.
/// </summary>
public record ObjectDrawingProperties : IDrawingProperties
{
    /// <inheritdoc />
    public Vector2 SourceSize => new(16f, 16f);

    /// <inheritdoc />
    public Vector2 Offset(float scaleSize)
    {
        return new(32f * scaleSize, 32f * scaleSize);
    }

    /// <inheritdoc />
    public Vector2 Origin(float scaleSize)
    {
        return new(8f * scaleSize, 8f * scaleSize);
    }

    /// <inheritdoc />
    public float RealScale(float scaleSize)
    {
        return 4f * scaleSize;
    }
}
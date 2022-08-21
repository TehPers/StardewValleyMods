using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// Properties about how the item is drawn in menus in the vanilla code.
/// </summary>
public interface IDrawingProperties
{
    /// <summary>
    /// The source size of the sprite.
    /// </summary>
    Vector2 SourceSize { get; }

    /// <summary>
    /// The offset the sprite is given when drawn by vanilla code.
    /// </summary>
    /// <param name="scaleSize">The scale size.</param>
    /// <returns>The vanilla offset of the sprite.</returns>
    Vector2 Offset(float scaleSize);

    /// <summary>
    /// The origin the sprite is given when drawn by vanilla code.
    /// </summary>
    /// <param name="scaleSize">The scale size.</param>
    /// <returns>The vanilla origin of the sprite.</returns>
    Vector2 Origin(float scaleSize);

    /// <summary>
    /// The actual scale used when the vanilla code draws the sprite.
    /// </summary>
    /// <param name="scaleSize">The scale size.</param>
    /// <returns>The scale that vanilla code actually uses.</returns>
    float RealScale(float scaleSize);
}

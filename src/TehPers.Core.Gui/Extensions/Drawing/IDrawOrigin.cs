using Microsoft.Xna.Framework;

namespace TehPers.Core.Gui.Extensions.Drawing;

/// <summary>
/// An origin to draw a sprite from.
/// </summary>
public interface IDrawOrigin
{
    /// <summary>
    /// Gets the offset of the top-left corner of the sprite.
    /// </summary>
    /// <param name="size">The size of the sprite.</param>
    /// <returns>The sprite's offset when being drawn.</returns>
    Vector2 GetTranslation(Vector2 size);
}

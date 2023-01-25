using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A UI label.
/// </summary>
public interface ILabel : IGuiComponent, IWithLayerDepth<ILabel>
{
    /// <summary>
    /// Sets the text of this label.
    /// </summary>
    /// <param name="text">The new text for the label.</param>
    /// <returns>The resulting component.</returns>
    ILabel WithText(string text);

    /// <summary>
    /// Sets the font this label is rendered with.
    /// </summary>
    /// <param name="font">The new font for the label.</param>
    /// <returns>The resulting component.</returns>
    ILabel WithFont(SpriteFont font);

    /// <summary>
    /// Sets the text color of this label.
    /// </summary>
    /// <param name="color">The new text color for the label.</param>
    /// <returns>The resulting component.</returns>
    ILabel WithColor(Color color);

    /// <summary>
    /// Sets the text scale of this label.
    /// </summary>
    /// <param name="scale">The new text scale for the label.</param>
    /// <returns>The resulting component.</returns>
    ILabel WithScale(Vector2 scale);

    /// <summary>
    /// Sets the text scale of this label.
    /// </summary>
    /// <param name="scale">The new text scale for the label (along both x and y).</param>
    /// <returns>The resulting component.</returns>
    ILabel WithScale(float scale)
    {
        return this.WithScale(new Vector2(scale, scale));
    }

    /// <summary>
    /// Sets the sprite effect of this label.
    /// </summary>
    /// <param name="spriteEffects">The new sprite effects for the label.</param>
    /// <returns>The resulting component.</returns>
    ILabel WithSpriteEffects(SpriteEffects spriteEffects);
}

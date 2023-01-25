using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A texture that is stretched to fill a space.
/// </summary>
public interface IStretchedTexture : IGuiComponent, IWithLayerDepth<IStretchedTexture>
{
    /// <summary>
    /// Sets the texture to draw.
    /// </summary>
    /// <param name="texture">The texture to draw.</param>
    /// <returns>The resulting component.</returns>
    IStretchedTexture WithTexture(Texture2D texture);

    /// <summary>
    /// Sets the source rectangle on the texture.
    /// </summary>
    /// <param name="sourceRectangle"></param>
    /// <returns>The resulting component.</returns>
    IStretchedTexture WithSourceRectangle(Rectangle? sourceRectangle);

    /// <summary>
    /// Sets the color to tint the texture.
    /// </summary>
    /// <param name="color">The color to tint the texture.</param>
    /// <returns>The resulting component.</returns>
    IStretchedTexture WithColor(Color color);

    /// <summary>
    /// Sets the sprite effects to apply to the texture.
    /// </summary>
    /// <param name="effects">The sprite effects to apply to the texture.</param>
    /// <returns>The resulting component.</returns>
    IStretchedTexture WithSpriteEffects(SpriteEffects effects);

    /// <summary>
    /// Sets the minimum scaled size of this texture. A min scaled width of 2 means that this
    /// texture must be stretched by at least double its original width, for example.
    /// </summary>
    /// <param name="minScale">The minimum scaled size of this texture.</param>
    /// <returns>The resulting component.</returns>
    IStretchedTexture WithMinScale(IGuiSize minScale);

    /// <summary>
    /// Sets the maximum scaled size of this texture. A max scaled width of 2 means that this
    /// texture can only be stretched up to double its original width, for example.
    /// </summary>
    /// <param name="maxScale">The maximum scaled size of this texture.</param>
    /// <returns>The resulting component.</returns>
    IStretchedTexture WithMaxScale(IPartialGuiSize maxScale);
}

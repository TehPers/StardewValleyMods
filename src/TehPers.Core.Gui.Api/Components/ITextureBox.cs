using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component which fills a space with a texture created from a 3x3 grid.
/// </summary>
public interface ITextureBox : IGuiComponent, IWithLayerDepth<ITextureBox>
{
    /// <summary>
    /// Sets the source texture to draw from.
    /// </summary>
    /// <param name="texture">The new source texture.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithTexture(Texture2D texture);

    /// <summary>
    /// Sets the source rectangle for the top left.
    /// </summary>
    /// <param name="topLeft">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithTopLeft(Rectangle? topLeft);

    /// <summary>
    /// Sets the source rectangle for the top center.
    /// </summary>
    /// <param name="topCenter">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithTopCenter(Rectangle? topCenter);

    /// <summary>
    /// Sets the source rectangle for the top right.
    /// </summary>
    /// <param name="topRight">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithTopRight(Rectangle? topRight);

    /// <summary>
    /// Sets the source rectangle for the center left.
    /// </summary>
    /// <param name="centerLeft">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithCenterLeft(Rectangle? centerLeft);

    /// <summary>
    /// Sets the source rectangle for the center.
    /// </summary>
    /// <param name="center">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithCenter(Rectangle? center);

    /// <summary>
    /// Sets the source rectangle for the center right.
    /// </summary>
    /// <param name="centerRight">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithCenterRight(Rectangle? centerRight);

    /// <summary>
    /// Sets the source rectangle for the bottom left.
    /// </summary>
    /// <param name="bottomLeft">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithBottomLeft(Rectangle? bottomLeft);

    /// <summary>
    /// Sets the source rectangle for the bottom center.
    /// </summary>
    /// <param name="bottomCenter">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithBottomCenter(Rectangle? bottomCenter);

    /// <summary>
    /// Sets the source rectangle for the bottom right.
    /// </summary>
    /// <param name="bottomRight">The new source rectangle.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithBottomRight(Rectangle? bottomRight);

    /// <summary>
    /// Sets the minimum scale of the individual texture parts.
    /// </summary>
    /// <param name="minScale">The new minimum scale.</param>
    /// <returns>The resulting component.</returns>
    ITextureBox WithMinScale(IGuiSize minScale);
}

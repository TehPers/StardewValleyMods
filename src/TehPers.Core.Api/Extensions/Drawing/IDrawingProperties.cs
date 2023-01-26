using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    /// <summary>
    /// Properties about how an item is drawn in menus in the vanilla code.
    /// </summary>
    public interface IDrawingProperties
    {
        /// <summary>
        /// The size of the source texture.
        /// </summary>
        Vector2 SourceSize { get; }

        /// <summary>
        /// The offset the sprite is given when drawn by the vanilla code.
        /// </summary>
        /// <param name="scaleSize">The scale size.</param>
        /// <returns>The vanilla offset.</returns>
        Vector2 Offset(float scaleSize);

        /// <summary>
        /// The origin the sprite is given when drawn by the vanilla code.
        /// </summary>
        /// <param name="scaleSize">The scale size.</param>
        /// <returns>The vanilla origin.</returns>
        Vector2 Origin(float scaleSize);

        /// <summary>
        /// The actual scale used when the vanilla code draws the sprite.
        /// </summary>
        /// <param name="scaleSize">The scale size.</param>
        /// <returns>The scale the vanilla code actually uses.</returns>
        float RealScale(float scaleSize);
    }
}
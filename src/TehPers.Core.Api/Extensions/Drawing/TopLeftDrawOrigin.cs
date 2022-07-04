using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    /// <summary>
    /// Positions the sprite such that the top-left of the sprite is at the location being drawn.
    /// </summary>
    public record TopLeftDrawOrigin() : DrawOrigin(Vector2.One, Vector2.Zero);
}

using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    /// <summary>
    /// Centers the sprite on the given position.
    /// </summary>
    public record CenterDrawOrigin() : DrawOrigin(Vector2.One * 2f, Vector2.One);
}

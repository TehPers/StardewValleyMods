using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal record CenterDrawOrigin() : DrawOrigin(Vector2.One * 2f, Vector2.One);
}
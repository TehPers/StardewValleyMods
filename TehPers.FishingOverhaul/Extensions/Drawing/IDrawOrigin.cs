using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal interface IDrawOrigin
    {
        Vector2 GetTranslation(Vector2 size);
    }
}
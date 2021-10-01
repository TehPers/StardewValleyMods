using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    public interface IDrawOrigin
    {
        Vector2 GetTranslation(Vector2 size);
    }
}
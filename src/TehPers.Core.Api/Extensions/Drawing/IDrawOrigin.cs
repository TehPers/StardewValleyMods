using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    public interface IDrawOrigin
    {
        Vector2 GetTranslation(Vector2 size);
    }
}
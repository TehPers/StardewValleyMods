using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.SpriteMain.Scalers
{
    internal interface IScaler
    {
        float Scale { get; }

        Rectangle DrawScaled(
            Texture2D texture,
            Rectangle source,
            Texture2D destTexture
        );
    }
}
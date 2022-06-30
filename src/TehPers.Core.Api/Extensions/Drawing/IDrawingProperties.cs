using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Extensions.Drawing
{
    public interface IDrawingProperties
    {
        Vector2 SourceSize { get; }

        Vector2 Offset(float scaleSize);

        Vector2 Origin(float scaleSize);

        float RealScale(float scaleSize);
    }
}
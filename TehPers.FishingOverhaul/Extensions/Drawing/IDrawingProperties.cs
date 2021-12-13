using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal interface IDrawingProperties
    {
        Vector2 SourceSize { get; }

        Vector2 Offset(float scaleSize);

        Vector2 Origin(float scaleSize);

        float RealScale(float scaleSize);
    }
}
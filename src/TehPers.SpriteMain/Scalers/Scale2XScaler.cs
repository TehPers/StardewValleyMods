using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.SpriteMain.Scalers
{
    internal class Scale2XScaler : GeneralScaler
    {
        public override float Scale => 2f;

        protected override Rectangle DrawScaled<T>(
            Texture2D texture,
            Rectangle source,
            Texture2D destTexture
        )
            where T : struct
        {
            var dest = new Rectangle(
                source.X * 2,
                source.Y * 2,
                source.Width * 2,
                source.Height * 2
            );
            if (source.Width == 0 || source.Height == 0)
            {
                return dest;
            }

            // Get input raw data
            var input = new T[source.Width * source.Height];
            texture.GetData(0, source, input, 0, input.Length);

            // Scale up the data
            var outputWidth = source.Width * 2;
            var outputHeight = source.Height * 2;
            var output = new T[outputWidth * outputHeight];
            for (var x = 0; x < source.Width; x++)
            {
                for (var y = 0; y < source.Height; y++)
                {
                    // Get source pixels
                    var srcP = input[y * source.Width + x];
                    var srcA = y > 0 ? input[(y - 1) * source.Width + x] : srcP;
                    var srcB = x < source.Width - 1 ? input[y * source.Width + (x + 1)] : srcP;
                    var srcC = x > 0 ? input[y * source.Width + (x - 1)] : srcP;
                    var srcD = y < source.Height - 1 ? input[(y + 1) * source.Width + x] : srcP;

                    // Calculate destination pixels
                    var abEqual = srcA.Equals(srcB);
                    var acEqual = srcA.Equals(srcC);
                    var bdEqual = srcB.Equals(srcD);
                    var cdEqual = srcC.Equals(srcD);
                    var dst1 = acEqual && cdEqual && abEqual ? srcA : srcP;
                    var dst2 = abEqual && acEqual && bdEqual ? srcB : srcP;
                    var dst3 = cdEqual && bdEqual && acEqual ? srcC : srcP;
                    var dst4 = bdEqual && abEqual && cdEqual ? srcD : srcP;

                    // Set destination pixels
                    output[(y * 2 + 0) * outputWidth + x * 2 + 0] = dst1;
                    output[(y * 2 + 0) * outputWidth + x * 2 + 1] = dst2;
                    output[(y * 2 + 1) * outputWidth + x * 2 + 0] = dst3;
                    output[(y * 2 + 1) * outputWidth + x * 2 + 1] = dst4;
                }
            }

            // Set output raw data
            destTexture.SetData(0, dest, output, 0, output.Length);
            return dest;
        }
    }
}
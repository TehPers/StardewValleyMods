using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.SpriteMain.Scalers
{
    internal class Scale3XScaler : GeneralScaler
    {
        public override float Scale => 3f;

        protected override Rectangle DrawScaled<T>(
            Texture2D texture,
            Rectangle source,
            Texture2D destTexture
        )
            where T : struct
        {
            var dest = new Rectangle(
                source.X * 3,
                source.Y * 3,
                source.Width * 3,
                source.Height * 3
            );
            if (source.Width == 0 || source.Height == 0)
            {
                return dest;
            }

            // Get input raw data
            var input = new T[source.Width * source.Height];
            texture.GetData(0, source, input, 0, input.Length);

            // Scale up the data
            var outputWidth = source.Width * 3;
            var outputHeight = source.Height * 3;
            var output = new T[outputWidth * outputHeight];
            for (var x = 0; x < source.Width; x++)
            {
                for (var y = 0; y < source.Height; y++)
                {
                    // Get source pixels
                    var srcE = input[y * source.Width + x];
                    var srcA = x > 0 && y > 0 ? input[(y - 1) * source.Width + (x - 1)] : srcE;
                    var srcB = y > 0 ? input[(y - 1) * source.Width + x] : srcE;
                    var srcC = x < source.Width - 1 && y > 0
                        ? input[(y - 1) * source.Width + x + 1]
                        : srcE;
                    var srcD = x > 0 ? input[y * source.Width + (x - 1)] : srcE;
                    var srcF = x < source.Width - 1 ? input[y * source.Width + x + 1] : srcE;
                    var srcG = x > 0 && y < source.Height - 1
                        ? input[(y + 1) * source.Width + (x - 1)]
                        : srcE;
                    var srcH = y < source.Height - 1 ? input[(y + 1) * source.Width + x] : srcE;
                    var srcI = x < source.Width - 1 && y < source.Height - 1
                        ? input[(y + 1) * source.Width + x + 1]
                        : srcE;

                    // Calculate destination pixels
                    var aeEqual = srcA.Equals(srcE);
                    var bdEqual = srcB.Equals(srcD);
                    var bfEqual = srcB.Equals(srcF);
                    var ceEqual = srcC.Equals(srcE);
                    var dhEqual = srcD.Equals(srcH);
                    var egEqual = srcE.Equals(srcG);
                    var eiEqual = srcE.Equals(srcI);
                    var fhEqual = srcF.Equals(srcH);
                    var dst1 = bdEqual && !dhEqual && !bfEqual ? srcD : srcE;
                    var dst2 =
                        bdEqual && !dhEqual && !bfEqual && !ceEqual
                        || bfEqual && !bdEqual && !fhEqual && !aeEqual
                            ? srcB
                            : srcE;
                    var dst3 = bfEqual && !bdEqual && !fhEqual ? srcF : srcE;
                    var dst4 =
                        dhEqual && !fhEqual && !bdEqual && !aeEqual
                        || bdEqual && !dhEqual && !bfEqual && !egEqual
                            ? srcD
                            : srcE;
                    var dst5 = srcE;
                    var dst6 =
                        bfEqual && !bdEqual && !fhEqual && !eiEqual
                        || fhEqual && !bfEqual && !dhEqual && !ceEqual
                            ? srcF
                            : srcE;
                    var dst7 = dhEqual && !fhEqual && !bdEqual ? srcD : srcE;
                    var dst8 =
                        fhEqual && !bfEqual && !dhEqual && !egEqual
                        || dhEqual && !fhEqual && !bdEqual && !eiEqual
                            ? srcH
                            : srcE;
                    var dst9 = fhEqual && !bfEqual && !dhEqual ? srcF : srcE;

                    // Set destination pixels
                    output[(y * 3 + 0) * outputWidth + x * 3 + 0] = dst1;
                    output[(y * 3 + 0) * outputWidth + x * 3 + 1] = dst2;
                    output[(y * 3 + 0) * outputWidth + x * 3 + 2] = dst3;
                    output[(y * 3 + 1) * outputWidth + x * 3 + 0] = dst4;
                    output[(y * 3 + 1) * outputWidth + x * 3 + 1] = dst5;
                    output[(y * 3 + 1) * outputWidth + x * 3 + 2] = dst6;
                    output[(y * 3 + 2) * outputWidth + x * 3 + 0] = dst7;
                    output[(y * 3 + 2) * outputWidth + x * 3 + 1] = dst8;
                    output[(y * 3 + 2) * outputWidth + x * 3 + 2] = dst9;
                }
            }

            // Set output raw data
            destTexture.SetData(0, dest, output, 0, output.Length);
            return dest;
        }
    }
}
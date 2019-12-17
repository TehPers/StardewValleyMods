using System.Text;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>Extension methods for <see cref="string"/>.</summary>
    public static class StringExtensions
    {
        /// <summary>Creates a string consisting of another string repeated many times.</summary>
        /// <param name="str">The string to repeat.</param>
        /// <param name="times">The number of times to repeat the string.</param>
        /// <returns>A string composed of the source repeated the given number of times.</returns>
        public static string Repeat(this string str, int times)
        {
            var sb = new StringBuilder();
            while (times-- > 0)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }
    }
}

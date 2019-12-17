using System;
using System.Globalization;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>Extensions for <see cref="Enum"/> types.</summary>
    public static class EnumExtensions
    {
        /// <summary>Checks if some flags are present in an enum value.</summary>
        /// <typeparam name="T">The type of <see cref="Enum"/>.</typeparam>
        /// <param name="source">The source value.</param>
        /// <param name="flags">The flags to check for.</param>
        /// <returns><see langword="true"/> if the flags are present, <see langword="false"/> otherwise.</returns>
        public static bool HasFlags<T>(this T source, T flags)
            where T : Enum
        {
            var sourceValue = Convert.ToInt64(source, CultureInfo.InvariantCulture);
            var flagsValue = Convert.ToInt64(flags, CultureInfo.InvariantCulture);
            return (sourceValue & flagsValue) == flagsValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Environment;

namespace TehPers.CoreMod.Api.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>Converts a <see cref="Season"/> to its lowercase string representation.</summary>
        /// <param name="season">The season to get the name of.</param>
        /// <returns>A lowercase string containing the season's name, or <c>null</c> if it does not represent exactly one season.</returns>
        public static string GetName(this Season season)
        {
            return season.Match<Season, string>()
                .When(Season.Spring, "spring")
                .When(Season.Summer, "summer")
                .When(Season.Fall, "fall")
                .When(Season.Winter, "winter")
                .Else((string)null);
        }

        /// <summary>Tries to convert a string into a <see cref="Season"/>.</summary>
        /// <param name="name">The name of the season.</param>
        /// <returns>The <see cref="Season"/> with the given name, or <c>null</c> if it didn't match.</returns>
        public static Season? GetSeason(this string name)
        {
            return name.ToUpper().Match<string, Season?>()
                .When("SPRING", Season.Spring)
                .When("SUMMER", Season.Summer)
                .When("FALL", Season.Fall)
                .When("WINTER", Season.Winter)
                .Else((Season?)null);
        }

        /// <summary>Tries to convert a string into a <see cref="Season"/>.</summary>
        /// <param name="name">The name of the season.</param>
        /// <param name="season">The resulting <see cref="Season"/>.</param>
        /// <returns>True if the name matched a season, false otherwise.</returns>
        public static bool TryGetSeason(this string name, out Season season)
        {
            Season? parsed = name.GetSeason();
            season = parsed ?? default;
            return parsed.HasValue;
        }

        /// <summary>Tries to convert a number representing a SDV water type to the equivalent <see cref="WaterTypes"/> enum value.</summary>
        /// <param name="sdvValue">A number representing a SDV water type.</param>
        /// <param name="waterTypes">The equivalent <see cref="WaterTypes"/> enum value, if successful.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryGetWaterTypes(this int sdvValue, out WaterTypes waterTypes)
        {
            bool success;
            (success, waterTypes) = sdvValue.Match<int, (bool success, WaterTypes types)>()
                .When(-1, () => (true, WaterTypes.Any))
                .When(0, () => (true, WaterTypes.River))
                .When(1, () => (true, WaterTypes.Lake))
                .Else((false, default));
            return success;
        }

        /// <summary>Gets the <see cref="Season"/> associated with the given date.</summary>
        /// <param name="date">The date to get the season of.</param>
        /// <returns>The <see cref="Season"/> which matches the given date.</returns>
        public static Season GetSeason(this SDate date)
        {
            return date.Season.GetSeason() ?? throw new InvalidOperationException($"Unexpected season {date.Season} when converting it to a Season enum value.");
        }

        /// <summary>Gets all the flags set within an enum value.</summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="source">The source value.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all the flags that were set.</returns>
        public static IEnumerable<T> GetFlags<T>(this T source) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Where(flag => source.HasFlag(flag));
        }

        /// <summary>Checks if an enum value contains any of a set of flags.</summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="source">The source value.</param>
        /// <param name="flags">The flags to check for.</param>
        /// <returns>True if any of the flags are in the source value, false otherwise.</returns>
        public static bool HasAnyFlags<T>(this T source, T flags) where T : Enum
        {
            var sourceConverted = Convert.ToUInt64(source);
            var flagsConverted = Convert.ToUInt64(flags);
            return (sourceConverted & flagsConverted) > 0;
        }
    }
}

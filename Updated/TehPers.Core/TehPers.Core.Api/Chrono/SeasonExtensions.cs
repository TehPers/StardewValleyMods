using System;
using System.Globalization;

namespace TehPers.Core.Api.Chrono
{
    /// <summary>Extensions for <see cref="Seasons"/>.</summary>
    public static class SeasonExtensions
    {
        /// <summary>Converts a <see cref="Seasons"/> to its lowercase string representation.</summary>
        /// <param name="season">The season to get the name of.</param>
        /// <returns>A lowercase string containing the season's name, or <c>null</c> if it does not represent exactly one season.</returns>
        public static string GetName(this Seasons season)
        {
            return season switch
            {
                Seasons.Spring => "spring",
                Seasons.Summer => "summer",
                Seasons.Fall => "fall",
                Seasons.Winter => "winter",
                _ => default,
            };
        }

        /// <summary>Tries to convert a string into a <see cref="Seasons"/>.</summary>
        /// <param name="name">The name of the season.</param>
        /// <returns>The <see cref="Seasons"/> with the given name, or <c>null</c> if it didn't match.</returns>
        public static Seasons? GetSeason(this string name)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));

            return name.ToUpper(CultureInfo.InvariantCulture) switch
            {
                "SPRING" => Seasons.Spring,
                "SUMMER" => Seasons.Summer,
                "FALL" => Seasons.Fall,
                "WINTER" => Seasons.Winter,
                _ => default(Seasons?),
            };
        }

        /// <summary>Tries to convert a string into a <see cref="Seasons"/>.</summary>
        /// <param name="name">The name of the season.</param>
        /// <param name="season">The resulting <see cref="Seasons"/>.</param>
        /// <returns>True if the name matched a season, false otherwise.</returns>
        public static bool TryGetSeason(this string name, out Seasons season)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));

            var parsed = name.GetSeason();
            season = parsed ?? default;
            return parsed.HasValue;
        }
    }
}

using System;
using StardewValley;

namespace TehPers.Core.Api.Chrono
{
    /// <summary>Represents a date and time in Stardew Valley.</summary>
    public readonly struct SDateTime : IEquatable<SDateTime>, IComparable<SDateTime>
    {
        /// <summary>Adds a <see cref="STimeSpan"/> to a <see cref="SDateTime"/>, producing a <see cref="SDateTime"/> representative of the result.</summary>
        /// <param name="first">The source <see cref="SDateTime"/>.</param>
        /// <param name="second">The <see cref="STimeSpan"/> indicating the interval between the source <see cref="SDateTime"/> and the result.</param>
        /// <returns>A <see cref="SDateTime"/> which is the given interval away in a positive direction from the source <see cref="SDateTime"/>.</returns>
        public static SDateTime operator +(in SDateTime first, in STimeSpan second)
        {
            return new SDateTime(0, 0, 0, first.TotalMinutes + second.TotalMinutes);
        }

        /// <summary>Subtracts a <see cref="STimeSpan"/> from a <see cref="SDateTime"/>, producing a <see cref="SDateTime"/> representative of the result.</summary>
        /// <param name="first">The source <see cref="SDateTime"/>.</param>
        /// <param name="second">The <see cref="STimeSpan"/> indicating the interval between the source <see cref="SDateTime"/> and the result.</param>
        /// <returns>A <see cref="SDateTime"/> which is the given interval away in a negative direction from the source <see cref="SDateTime"/>.</returns>
        public static SDateTime operator -(in SDateTime first, in STimeSpan second)
        {
            return new SDateTime(0, 0, 0, first.TotalMinutes - second.TotalMinutes);
        }

        /// <summary>Calculates the interval between two instances of <see cref="SDateTime"/>.</summary>
        /// <param name="first">The lower <see cref="SDateTime"/> of the interval being calculated.</param>
        /// <param name="second">The higher <see cref="SDateTime"/> of the interval being calculated.</param>
        /// <returns>A <see cref="STimeSpan"/> which indicates the interval between the given instances of <see cref="SDateTime"/>.</returns>
        public static STimeSpan operator -(in SDateTime first, in SDateTime second)
        {
            return new STimeSpan(first.TotalMinutes - second.TotalMinutes);
        }

        /// <summary>Checks if the first argument comes after the second argument in chronological order.</summary>
        /// <param name="first">The first <see cref="SDateTime"/>.</param>
        /// <param name="second">The second <see cref="SDateTime"/>.</param>
        /// <returns><see langword="true"/> if the first <see cref="SDateTime"/> is chronologally after the second <see cref="SDateTime"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator >(in SDateTime first, in SDateTime second)
        {
            return first.CompareTo(second) > 0;
        }

        /// <summary>Checks if the first argument comes after the second argument in chronological order, or if they are equal.</summary>
        /// <param name="first">The first <see cref="SDateTime"/>.</param>
        /// <param name="second">The second <see cref="SDateTime"/>.</param>
        /// <returns><see langword="true"/> if the first <see cref="SDateTime"/> is chronologally equivalent or after the second <see cref="SDateTime"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator >=(in SDateTime first, in SDateTime second)
        {
            return first.CompareTo(second) >= 0;
        }

        /// <summary>Checks if the first argument comes before the second argument in chronological order.</summary>
        /// <param name="first">The first <see cref="SDateTime"/>.</param>
        /// <param name="second">The second <see cref="SDateTime"/>.</param>
        /// <returns><see langword="true"/> if the first <see cref="SDateTime"/> is chronologally before the second <see cref="SDateTime"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator <(in SDateTime first, in SDateTime second)
        {
            return first.CompareTo(second) < 0;
        }

        /// <summary>Checks if the first argument comes before the second argument in chronological order, or if they are equal.</summary>
        /// <param name="first">The first <see cref="SDateTime"/>.</param>
        /// <param name="second">The second <see cref="SDateTime"/>.</param>
        /// <returns><see langword="true"/> if the first <see cref="SDateTime"/> is chronologally equivalent or before the second <see cref="SDateTime"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator <=(in SDateTime first, in SDateTime second)
        {
            return first.CompareTo(second) <= 0;
        }

        /// <summary>Checks if two <see cref="SDateTime"/> instances are equivalent.</summary>
        /// <param name="first">The first <see cref="SDateTime"/>.</param>
        /// <param name="second">The second <see cref="SDateTime"/>.</param>
        /// <returns><see langword="true"/> if the two <see cref="SDateTime"/> instances are equivalent, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(in SDateTime first, in SDateTime second)
        {
            return first.Equals(second);
        }

        /// <summary>Checks if two <see cref="SDateTime"/> instances are not equivalent.</summary>
        /// <param name="first">The first <see cref="SDateTime"/>.</param>
        /// <param name="second">The second <see cref="SDateTime"/>.</param>
        /// <returns><see langword="true"/> if the two <see cref="SDateTime"/> instances are not equivalent, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(in SDateTime first, in SDateTime second)
        {
            return !first.Equals(second);
        }

        /// <summary>Creates a <see cref="SDateTime"/> from a date and time of day.</summary>
        /// <param name="years">The number of years to add to the <see cref="SDateTime"/>.</param>
        /// <param name="season">The current season of the <see cref="SDateTime"/>.</param>
        /// <param name="days">The number of days to add to the <see cref="SDateTime"/>.</param>
        /// <param name="timeOfDay">The time of day, in the format 'hhmm'. For example, 11:20 would be <c>1120</c>.</param>
        /// <returns>The <see cref="SDateTime"/>.</returns>
        public static SDateTime FromDateAndTime(int years, Seasons season, int days, int timeOfDay)
        {
            return new SDateTime(years, season, days, 60 * (timeOfDay / 100) + timeOfDay % 100);
        }

        /// <summary>Gets the current date and time.</summary>
        public static SDateTime Now => SDateTime.FromDateAndTime(Game1.year, Game1.currentSeason?.GetSeason() ?? Seasons.Spring, Game1.dayOfMonth, Game1.timeOfDay);

        /// <summary>Gets the current date.</summary>
        public static SDateTime Today => new SDateTime(Game1.year, Game1.currentSeason?.GetSeason() ?? Seasons.Spring, Game1.dayOfMonth);

        /// <summary>Gets the total number of elapsed years.</summary>
        public float TotalYears => this.TotalMinutes / (4f * 28f * 2400f);

        /// <summary>Gets the total number of elapsed seasons.</summary>
        public float TotalSeasons => this.TotalMinutes / (28f * 2400f);

        /// <summary>Gets the total number of elapsed days.</summary>
        public float TotalDays => this.TotalMinutes / 2400f;

        /// <summary>Gets the total number of elapsed minutes.</summary>
        public int TotalMinutes { get; }

        /// <summary>Gets the number of elapsed years.</summary>
        public int Year => (int)this.TotalYears + 1;

        /// <summary>Gets the number of elapsed seasons in the year.</summary>
        public Seasons Season =>
            ((int)this.TotalSeasons % 4) switch
            {
                0 => Seasons.Spring,
                1 => Seasons.Summer,
                2 => Seasons.Fall,
                3 => Seasons.Winter,
                _ => throw new InvalidOperationException(),
            };

        /// <summary>Gets the number of elapsed days in the season.</summary>
        public int DayOfSeason => (int)this.TotalDays % 28;

        /// <summary>Gets the current time of day in SDV format (HHmm).</summary>
        public int TimeOfDay => this.TotalMinutes + 40 * (this.TotalMinutes / 60);

        /// <summary>Gets the number of elapsed minutes in the day. This is not in SDV time format, use <see cref="TimeOfDay"/> instead if that is needed.</summary>
        public int MinutesOfDay => this.TotalMinutes % 2400;

        /// <summary>Initializes a new instance of the <see cref="SDateTime"/> struct.</summary>
        /// <param name="years">The number of years to add to the <see cref="SDateTime"/>.</param>
        /// <param name="season">The current season of the <see cref="SDateTime"/>.</param>
        /// <param name="days">The number of days to add to the <see cref="SDateTime"/>.</param>
        /// <param name="minutes">The number of minutes to add to the <see cref="SDateTime"/>.</param>
        public SDateTime(int years, Seasons season, int days = 0, int minutes = 0)
        {
            var seasons = season switch
            {
                Seasons.Spring => 0,
                Seasons.Summer => 1,
                Seasons.Fall => 2,
                Seasons.Winter => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(season)),
            };

            this.TotalMinutes = minutes + days * 2400 + seasons * 28 * 2400 + years * 4 * 28 * 2400;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDateTime"/> struct.
        /// </summary>
        /// <param name="years">The number of years to add to the <see cref="SDateTime"/>.</param>
        /// <param name="seasons">The number of seasons to add to the <see cref="SDateTime"/>.</param>
        /// <param name="days">The number of days to add to the <see cref="SDateTime"/>.</param>
        /// <param name="minutes">The number of minutes to add to the <see cref="SDateTime"/>.</param>
        public SDateTime(int years = 0, int seasons = 0, int days = 0, int minutes = 0)
        {
            this.TotalMinutes = minutes + days * 2400 + seasons * 28 * 2400 + years * 4 * 28 * 2400;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SDateTime other && this.Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(SDateTime other)
        {
            return this.TotalMinutes == other.TotalMinutes;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.TotalMinutes;
        }

        /// <inheritdoc />
        public int CompareTo(SDateTime other)
        {
            return this.TotalMinutes.CompareTo(other.TotalMinutes);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var timeOfDay = this.TimeOfDay;
            return $"{this.Season} {this.DayOfSeason}, {this.Year} {timeOfDay / 100}:{timeOfDay % 100}";
        }
    }
}
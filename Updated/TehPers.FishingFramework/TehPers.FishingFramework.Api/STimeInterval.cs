using System;
using System.ComponentModel;
using TehPers.Core.Api.Chrono;
using TehPers.Core.Api.Json;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// An interval of time.
    /// </summary>
    [JsonDescribe]
    public readonly struct STimeInterval : IEquatable<STimeInterval>
    {
        /// <summary>
        /// Checks if two instances of <see cref="STimeInterval"/> are equal.
        /// </summary>
        /// <param name="left">The first interval.</param>
        /// <param name="right">The second interval.</param>
        /// <returns>Whether the two intervals are equal.</returns>
        public static bool operator ==(STimeInterval left, STimeInterval right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks if two instances of <see cref="STimeInterval"/> are not equal.
        /// </summary>
        /// <param name="left">The first interval.</param>
        /// <param name="right">The second interval.</param>
        /// <returns>Whether the two intervals are inequal.</returns>
        public static bool operator !=(STimeInterval left, STimeInterval right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets the (inclusive) earliest time in this interval.
        /// </summary>
        [Description("The earliest time in this interval.")]
        public int Start { get; }

        /// <summary>
        /// Gets the (exclusive) latest time in this interval.
        /// </summary>
        [Description("The latest time in this interval.")]
        public int Finish { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="STimeInterval"/> struct.
        /// </summary>
        /// <param name="start">The (inclusive) earliest time in this interval.</param>
        /// <param name="finish">The (exclusive) latest time in this interval.</param>
        public STimeInterval(int start, int finish)
        {
            this.Start = start;
            this.Finish = finish;
        }

        /// <summary>
        /// Checks if a time is contained in this interval.
        /// </summary>
        /// <param name="timeOfDay">The time of day. For example, 6:00am = 600, noon = 1200, midnight = 2400, 2:00am = 2600, etc.</param>
        /// <returns>Whether the time is contained in this interval.</returns>
        public bool Contains(int timeOfDay)
        {
            return timeOfDay >= this.Start && timeOfDay < this.Finish;
        }

        /// <summary>
        /// Checks if a time is contained in this interval.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the time is contained in this interval.</returns>
        public bool Contains(SDateTime time)
        {
            return this.Contains(time.TimeOfDay);
        }

        /// <inheritdoc />
        public bool Equals(STimeInterval other)
        {
            return this.Start == other.Start && this.Finish == other.Finish;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is STimeInterval other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return unchecked((this.Start * 397) ^ this.Finish);
        }
    }
}
using System;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct STimeSpan : IComparable<STimeSpan> {
        /// <summary>The total number of elapsed years.</summary>
        public float TotalYears => (float) this.TotalMinutes / (4f * 28f * 2400f);

        /// <summary>The total number of elapsed seasons.</summary>
        public float TotalSeasons => (float) this.TotalMinutes / (28f * 2400f);

        /// <summary>The total number of elapsed days.</summary>
        public float TotalDays => (float) this.TotalMinutes / 2400f;

        /// <summary>The total number of elapsed minutes.</summary>
        public int TotalMinutes { get; }

        public STimeSpan(int minutes, int days = 0, int seasons = 0, int years = 0) {
            this.TotalMinutes = minutes + days * 2400 + seasons * 28 * 2400 + years * 4 * 28 * 2400;
        }

        /// <inheritdoc />
        public int CompareTo(STimeSpan other) {
            return this.TotalMinutes.CompareTo(other.TotalMinutes);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is STimeSpan other && this.Equals(other);
        }

        public bool Equals(STimeSpan other) {
            return this.CompareTo(other) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.TotalMinutes;
        }

        public static STimeSpan operator +(STimeSpan first, STimeSpan second) => new STimeSpan(first.TotalMinutes + second.TotalMinutes);
        public static STimeSpan operator -(STimeSpan first, STimeSpan second) => new STimeSpan(first.TotalMinutes + second.TotalMinutes);
        public static bool operator >(STimeSpan first, STimeSpan second) => first.CompareTo(second) > 0;
        public static bool operator >=(STimeSpan first, STimeSpan second) => first.CompareTo(second) >= 0;
        public static bool operator <(STimeSpan first, STimeSpan second) => first.CompareTo(second) < 0;
        public static bool operator <=(STimeSpan first, STimeSpan second) => first.CompareTo(second) <= 0;
        public static bool operator ==(STimeSpan first, STimeSpan second) => first.Equals(second);
        public static bool operator !=(STimeSpan first, STimeSpan second) => !first.Equals(second);
    }
}
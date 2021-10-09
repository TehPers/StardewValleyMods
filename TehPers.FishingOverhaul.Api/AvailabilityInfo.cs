using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Information about the availability of a catchable item.
    /// </summary>
    [JsonDescribe]
    public record AvailabilityInfo(
        [property: JsonRequired]
        [property:
            Description(
                "The base chance this will be caught. This is not a percentage chance, but rather a "
                + "weight relative to all available entries."
            )]
        double BaseChance
    )
    {
        [Description("Time this becomes available (inclusive).")]
        [DefaultValue(600)]
        public int StartTime { get; init; } = 600;

        [Description("Time this is no longer available (exclusive).")]
        [DefaultValue(2600)]
        public int EndTime { get; init; } = 2600;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="Seasons"/>.
        /// </summary>
        [JsonProperty(nameof(AvailabilityInfo.Seasons))]
        [Description("Seasons this can be caught in. Default is all.")]
        public IEnumerable<Seasons> SeasonsSplit
        {
            get => this.Seasons.Split();
            init => this.Seasons = value.Join();
        }

        [JsonIgnore]
        public Seasons Seasons { get; init; } = Seasons.All;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="Weathers"/>.
        /// </summary>
        [JsonProperty(nameof(AvailabilityInfo.Weathers))]
        [Description("Weathers this can be caught in. Default is all.")]
        public IEnumerable<Weathers> WeathersSplit
        {
            get => this.Weathers.Split();
            init => this.Weathers = value.Join();
        }

        [JsonIgnore]
        public Weathers Weathers { get; init; } = Weathers.All;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="WaterTypes"/>.
        /// </summary>
        [JsonProperty(nameof(AvailabilityInfo.WaterTypes))]
        [Description(
            "The type of water this can be caught in. Each location handles this differently. "
            + "Default is all."
        )]
        public IEnumerable<WaterTypes> WaterTypesSplit
        {
            get => this.WaterTypes.Split();
            init => this.WaterTypes = value.Join();
        }

        [JsonIgnore]
        public WaterTypes WaterTypes { get; set; } = WaterTypes.All;

        [Description("Required fishing level to see this.")]
        [DefaultValue(0)]
        public int MinFishingLevel { get; set; } = 0;

        [Description("Maximum fishing level required to see this, or null for no max.")]
        [DefaultValue(null)]
        public int? MaxFishingLevel { get; set; } = null;

        [Description(
            "List of locations this should be available in. Leaving this empty will make this "
            + "available everywhere. Some locations have special handling. For example, the mines "
            + "use the location names 'UndergroundMine' and 'UndergroundMine/N', where N is the "
            + "floor number (both location names are valid for the floor)."
        )]
        public ImmutableArray<string> IncludeLocations { get; init; } =
            ImmutableArray<string>.Empty;

        [Description(
            "List of locations this should not be available in. This takes priority over "
            + nameof(AvailabilityInfo.IncludeLocations)
            + "."
        )]
        public ImmutableArray<string> ExcludeLocations { get; init; } =
            ImmutableArray<string>.Empty;

        [Description("Content Patcher conditions for when this is available.")]
        public ImmutableDictionary<string, string> When { get; init; } =
            ImmutableDictionary<string, string>.Empty;

        /// <summary>
        /// Gets the weighted chance of this being caught, if any. This does not test the
        /// conditions in <see cref="When"/>.
        /// </summary>
        /// <param name="time">The time of day.</param>
        /// <param name="seasons">The seasons to check.</param>
        /// <param name="weathers">The weathers to check.</param>
        /// <param name="fishingLevel">The user's fishing level.</param>
        /// <param name="locations">The locations to check.</param>
        /// <param name="waterTypes">The water tyes to check.</param>
        /// <param name="depth">The bobber depth.</param>
        /// <returns>The weighted chance of this being caught, or <see langword="null"/> if not available.</returns>
        public virtual double? GetWeightedChance(
            int time,
            Seasons seasons,
            Weathers weathers,
            int fishingLevel,
            IEnumerable<string> locations,
            WaterTypes waterTypes = WaterTypes.All,
            int depth = 4
        )
        {
            // Verify time is valid
            if (time < this.StartTime || time >= this.EndTime)
            {
                return null;
            }

            // Verify season is valid
            if ((this.Seasons & seasons) == Seasons.None)
            {
                return null;
            }

            // Verify weather is valid
            if ((this.Weathers & weathers) == Weathers.None)
            {
                return null;
            }

            // Verify water type is valid
            if ((this.WaterTypes & waterTypes) == WaterTypes.None)
            {
                return null;
            }

            // Verify fishing level is valid
            if (fishingLevel < this.MinFishingLevel
                || this.MaxFishingLevel is { } maxLevel && fishingLevel > maxLevel)
            {
                return null;
            }

            // Verify location is valid
            var ignoreIncluded = !this.IncludeLocations.Any();
            var validLocation = locations.Aggregate(
                (bool?)null,
                (valid, cur) => valid is not false
                    && !this.ExcludeLocations.Contains(cur)
                    && (ignoreIncluded || this.IncludeLocations.Contains(cur))
            );
            if (validLocation is not true)
            {
                return null;
            }

            // Calculate spawn weight
            return this.BaseChance;
        }
    }
}
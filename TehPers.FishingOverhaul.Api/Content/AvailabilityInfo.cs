using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
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
            get => this.Seasons switch
            {
                Seasons.All => new[] { Seasons.All },
                _ => new[] { Seasons.Spring, Seasons.Summer, Seasons.Fall, Seasons.Winter }.Where(
                    s => this.Seasons.HasFlag(s)
                ),
            };
            init => this.Seasons = value.Aggregate(Seasons.None, (acc, cur) => acc | cur);
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
            get => this.Weathers switch
            {
                Weathers.All => new[] { Weathers.All },
                _ => new[] { Weathers.Sunny, Weathers.Rainy }.Where(w => this.Weathers.HasFlag(w)),
            };
            init => this.Weathers = value.Aggregate(Weathers.None, (acc, cur) => acc | cur);
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
            get => this.WaterTypes switch
            {
                WaterTypes.All => new[] { WaterTypes.All },
                _ => new[] { WaterTypes.River, WaterTypes.PondOrOcean, WaterTypes.Freshwater }
                    .Where(w => this.WaterTypes.HasFlag(w)),
            };
            init => this.WaterTypes = value.Aggregate(WaterTypes.None, (acc, cur) => acc | cur);
        }

        [JsonIgnore]
        public WaterTypes WaterTypes { get; init; } = WaterTypes.All;

        [Description("Required fishing level to see this.")]
        [DefaultValue(0)]
        public int MinFishingLevel { get; init; } = 0;

        [Description("Maximum fishing level required to see this, or null for no max.")]
        [DefaultValue(null)]
        public int? MaxFishingLevel { get; init; } = null;

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

        [Description("Constraints on the position of the bobber in the map when fishing.")]
        public PositionConstraint Position { get; init; } = new();

        [Description("Content Patcher conditions for when this is available.")]
        public ImmutableDictionary<string, string> When { get; init; } =
            ImmutableDictionary<string, string>.Empty;

        /// <summary>
        /// Gets the weighted chance of this being caught, if any. This does not test the
        /// conditions in <see cref="When"/>.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <returns>The weighted chance of this being caught, or <see langword="null"/> if not available.</returns>
        public virtual double? GetWeightedChance(FishingInfo fishingInfo)
        {
            // Verify time is valid
            if (fishingInfo.Time < this.StartTime || fishingInfo.Time >= this.EndTime)
            {
                return null;
            }

            // Verify season is valid
            if ((this.Seasons & fishingInfo.Seasons) == Seasons.None)
            {
                return null;
            }

            // Verify weather is valid
            if ((this.Weathers & fishingInfo.Weathers) == Weathers.None)
            {
                return null;
            }

            // Verify water type is valid
            if ((this.WaterTypes & fishingInfo.WaterTypes) == WaterTypes.None)
            {
                return null;
            }

            // Verify fishing level is valid
            if (fishingInfo.FishingLevel < this.MinFishingLevel
                || this.MaxFishingLevel is { } maxLevel && fishingInfo.FishingLevel > maxLevel)
            {
                return null;
            }

            // Verify location is valid
            var ignoreIncluded = !this.IncludeLocations.Any();
            var validLocation = fishingInfo.Locations.Aggregate(
                (bool?)null,
                (valid, cur) => valid is not false
                    && !this.ExcludeLocations.Contains(cur)
                    && (ignoreIncluded || this.IncludeLocations.Contains(cur))
            );
            if (validLocation is not true)
            {
                return null;
            }

            // Verify position is valid
            if (!this.Position.Matches(fishingInfo.BobberPosition))
            {
                return null;
            }

            // Calculate spawn weight
            return this.BaseChance;
        }
    }
}
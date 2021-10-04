using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class Availability
    {
        [JsonRequired]
        [Description(
            "The base chance this will be caught. This is not a percentage chance, but rather a "
            + "weight relative to all available entries."
        )]
        public double BaseChance { get; set; }

        [Description("Time this becomes available (inclusive).")]
        [DefaultValue(600)]
        public int StartTime { get; set; } = 600;

        [Description("Time this is no longer available (exclusive).")]
        [DefaultValue(2600)]
        public int EndTime { get; set; } = 2600;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="Seasons"/>.
        /// </summary>
        [JsonProperty(nameof(Availability.Seasons))]
        [Description("Seasons this can be caught in. Default is all.")]
        public IEnumerable<Seasons> SeasonsSplit
        {
            get => this.Seasons.Split();
            set => this.Seasons = value.Join();
        }

        [JsonIgnore]
        public Seasons Seasons { get; set; } = Seasons.All;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="Weathers"/>.
        /// </summary>
        [JsonProperty(nameof(Availability.Weathers))]
        [Description("Weathers this can be caught in. Default is all.")]
        public IEnumerable<Weathers> WeathersSplit
        {
            get => this.Weathers.Split();
            set => this.Weathers = value.Join();
        }

        [JsonIgnore]
        public Weathers Weathers { get; set; } = Weathers.All;

        /// <summary>
        /// Used for JSON serialization. Prefer <see cref="WaterTypes"/>.
        /// </summary>
        [JsonProperty(nameof(Availability.WaterTypes))]
        [Description(
            "The type of water this can be caught in. Each location handles this differently. "
            + "Default is all."
        )]
        public IEnumerable<WaterTypes> WaterTypesSplit
        {
            get => this.WaterTypes.Split();
            set => this.WaterTypes = value.Join();
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
        public List<string> IncludeLocations { get; init; } = new();

        [Description(
            "List of locations this should not be available in. This takes priority over "
            + nameof(Availability.IncludeLocations)
            + "."
        )]
        public List<string> ExcludeLocations { get; init; } = new();

        [JsonConstructor]
        private Availability()
        {
            // Used for JSON deserialization
        }

        public Availability(double baseChance)
        {
            this.BaseChance = baseChance;
        }

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
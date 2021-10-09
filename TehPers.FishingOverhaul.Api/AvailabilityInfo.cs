using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class AvailabilityInfo
    {
        [JsonRequired]
        [Description(
            "The base chance this will be caught. This is not a percentage chance, but rather a "
            + "weight relative to all available entries."
        )]
        public double BaseChance { get; set; }

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
            + nameof(AvailabilityInfo.IncludeLocations)
            + "."
        )]
        public List<string> ExcludeLocations { get; init; } = new();

        [Description("Content Patcher conditions for when this is available.")]
        public Dictionary<string, string> When { get; init; } = new();

        [JsonConstructor]
        private AvailabilityInfo()
        {
            // Used for JSON deserialization
        }

        public AvailabilityInfo(double baseChance)
        {
            this.BaseChance = baseChance;
        }

        public virtual double? GetWeightedChance(
            int fishingLevel,
            IEnumerable<string> locations,
            WaterTypes waterTypes = WaterTypes.All,
            int depth = 4
        )
        {
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
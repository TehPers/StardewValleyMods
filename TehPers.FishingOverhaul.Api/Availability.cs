using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class Availability
    {
        [Description(
            "The chance this will be caught. This is not a percentage chance, but rather a weight relative to all available treasure."
        )]
        public double WeightedChance { get; set; }

        [Description("Time this becomes available (inclusive).")]
        public int StartTime { get; set; }

        [Description("Time this is no longer available (exclusive).")]
        public int EndTime { get; set; }

        [Description("Seasons this can be caught in.")]
        public Seasons Seasons { get; set; }

        [Description("Weathers this can be caught in.")]
        public Weathers Weathers { get; set; }

        [Description(
            "The type of water this can be caught in. Each location handles this differently."
        )]
        public WaterTypes WaterTypes { get; set; }

        [Description("Required fishing level to see this.")]
        public int MinFishingLevel { get; set; }

        [Description("Maximum fishing level required to see this, or null for no max.")]
        public int? MaxFishingLevel { get; set; }

        [Description(
            "List of locations this should be available in. Leaving this empty will make this "
            + "available everywhere."
        )]
        public List<string> IncludeLocations { get; set; }

        [Description(
            "List of locations this should not be available in. This takes priority over "
            + nameof(Availability.IncludeLocations)
            + "."
        )]
        public List<string> ExcludeLocations { get; set; }

        [JsonConstructor]
        public Availability(
            double weightedChance,
            int startTime = 600,
            int endTime = 2600,
            Seasons seasons = Seasons.All,
            Weathers weathers = Weathers.All,
            WaterTypes waterTypes = WaterTypes.All,
            int minFishingLevel = 0,
            int? maxFishingLevel = null,
            List<string>? includeLocations = null,
            List<string>? excludeLocations = null
        )
        {
            this.WeightedChance = weightedChance;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Seasons = seasons;
            this.Weathers = weathers;
            this.WaterTypes = waterTypes;
            this.MinFishingLevel = minFishingLevel;
            this.MaxFishingLevel = maxFishingLevel;
            this.IncludeLocations = includeLocations ?? new();
            this.ExcludeLocations = excludeLocations ?? new();
        }

        public virtual double? GetWeightedChance(
            int time,
            Seasons season,
            Weathers weather,
            int fishingLevel,
            WaterTypes waterTypes = WaterTypes.All
        )
        {
            // Verify time is valid
            if (time < this.StartTime || time >= this.EndTime)
            {
                return null;
            }

            // Verify season is valid
            if ((this.Seasons & season) == Seasons.None)
            {
                return null;
            }

            // Verify weather is valid
            if ((this.Weathers & weather) == Weathers.None)
            {
                return null;
            }

            // Verify water type is valid
            if ((this.WaterTypes & waterTypes) == WaterTypes.None)
            {
                return null;
            }

            // Verify fishing level is valid
            if (fishingLevel < this.MinFishingLevel || this.MaxFishingLevel is { } maxLevel && fishingLevel > maxLevel)
            {
                return null;
            }

            // Calculate spawn weight
            return this.WeightedChance;
        }
    }
}
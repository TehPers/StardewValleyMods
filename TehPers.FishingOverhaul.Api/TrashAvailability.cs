using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public readonly struct TrashAvailability
    {
        [JsonRequired]
        [Description("The item key.")]
        public NamespacedKey ItemKey { get; }

        [JsonRequired]
        [Description(
            "The chance this trash will be caught. This is not a percentage chance, but rather a weight relative to all available trash."
        )]
        public double WeightedChance { get; }

        [Description("Time the trash becomes available (inclusive).")]
        public int StartTime { get; }

        [Description("Time the trash is no longer available (exclusive).")]
        public int EndTime { get; }

        [Description("Seasons the trash can be caught in.")]
        public Seasons Seasons { get; }

        [Description("Weathers the trash can be caught in.")]
        public Weathers Weathers { get; }

        [Description("The type of water this trash can be caught in. Each location handles this differently.")]
        public WaterTypes WaterTypes { get; }

        [Description("Required fishing level to see this trash.")]
        public int MinFishingLevel { get; }

        [Description(
            "List of locations the trash should be available in. Leaving this empty will make the trash available everywhere."
        )]
        public List<string> IncludeLocations { get; }

        [Description(
            "List of locations the trash should not be available in. This takes priority over "
            + nameof(TrashAvailability.IncludeLocations)
            + "."
        )]
        public List<string> ExcludeLocations { get; }

        [JsonConstructor]
        public TrashAvailability(
            NamespacedKey itemKey,
            double weightedChance,
            int startTime = 600,
            int endTime = 2600,
            Seasons seasons = Seasons.All,
            Weathers weathers = Weathers.All,
            WaterTypes waterTypes = WaterTypes.All,
            int minFishingLevel = 0,
            List<string>? includeLocations = null,
            List<string>? excludeLocations = null
        )
        {
            this.ItemKey = itemKey;
            this.WeightedChance = weightedChance;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Seasons = seasons;
            this.Weathers = weathers;
            this.WaterTypes = waterTypes;
            this.MinFishingLevel = minFishingLevel;
            this.IncludeLocations = includeLocations ?? new();
            this.ExcludeLocations = includeLocations ?? new();
        }

        public double? GetWeightedChance(
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
            if (fishingLevel < this.MinFishingLevel)
            {
                return null;
            }

            // Calculate spawn weight
            return this.WeightedChance;
        }
    }
}
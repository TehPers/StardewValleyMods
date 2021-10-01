using System;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public readonly struct FishAvailability
    {
        [JsonRequired]
        [Description("The item key.")]
        public NamespacedKey FishKey { get; }

        [Description("Time the fish becomes available (inclusive).")]
        public int StartTime { get; }

        [Description("Time the fish is no longer available (exclusive).")]
        public int EndTime { get; }

        [Description("Seasons the fish can be caught in.")]
        public Seasons Seasons { get; }

        [Description("Weathers the fish can be caught in.")]
        public Weathers Weathers { get; }

        [Description("The type of water this fish can be caught in. Each location handles this differently.")]
        public WaterTypes WaterTypes { get; }

        [Description("The required fishing depth to maximize the chances of catching the fish.")]
        public int MaxDepth { get; }

        [Description("Base chance of finding this fish.")]
        public double SpawnMultiplier { get; }

        [Description(
            "Effect that sending the bobber by less than the max distance has on the chance. This value should be no more than 1."
        )]
        public double DepthMultiplier { get; }

        [Description("Required fishing level to see this fish.")]
        public int MinFishingLevel { get; }

        [JsonConstructor]
        public FishAvailability(
            NamespacedKey fishKey,
            int startTime = 600,
            int endTime = 2600,
            Seasons seasons = Seasons.All,
            Weathers weathers = Weathers.All,
            WaterTypes waterTypes = WaterTypes.All,
            int maxDepth = 4,
            double spawnMultiplier = 0.3,
            double depthMultiplier = 0.5,
            int minFishingLevel = 0
        )
        {
            this.FishKey = fishKey;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Seasons = seasons;
            this.Weathers = weathers;
            this.WaterTypes = waterTypes;
            this.MaxDepth = maxDepth;
            this.SpawnMultiplier = spawnMultiplier;
            this.DepthMultiplier = depthMultiplier;
            this.MinFishingLevel = minFishingLevel;
        }

        public double? GetWeightedChance(
            int time,
            Seasons season,
            Weathers weather,
            double depth,
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
            return this.SpawnMultiplier * (1 - Math.Max(0, this.MaxDepth - depth) * this.DepthMultiplier)
                   + fishingLevel / 50.0f;
        }
    }
}
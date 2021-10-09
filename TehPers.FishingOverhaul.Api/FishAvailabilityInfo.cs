using System;
using System.Collections.Generic;
using System.ComponentModel;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Information about the availability of a catchable fish.
    /// </summary>
    /// <inheritdoc cref="AvailabilityInfo"/>
    [JsonDescribe]
    public record FishAvailabilityInfo(double BaseChance) : AvailabilityInfo(BaseChance)
    {
        [Description(
            "Effect that sending the bobber by less than the max distance has on the chance. This "
            + "value should be no more than 1."
        )]
        [DefaultValue(0.1d)]
        public double DepthMultiplier { get; init; } = 0.1d;

        [Description("The required fishing depth to maximize the chances of catching the fish.")]
        [DefaultValue(4)]
        public int MaxDepth { get; init; } = 4;

        public override double? GetWeightedChance(
            int time,
            Seasons seasons,
            Weathers weathers,
            int fishingLevel,
            IEnumerable<string> locations,
            WaterTypes waterTypes = WaterTypes.All,
            int depth = 4
        )
        {
            return base
                .GetWeightedChance(
                    time,
                    seasons,
                    weathers,
                    fishingLevel,
                    locations,
                    waterTypes,
                    depth
                )
                .Select(
                    baseChance =>
                        baseChance * (1 - Math.Max(0, this.MaxDepth - depth) * this.DepthMultiplier)
                        + fishingLevel / 50.0f
                );
        }
    }
}
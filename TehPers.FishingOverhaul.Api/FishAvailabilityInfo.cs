using System;
using System.Collections.Generic;
using System.ComponentModel;
using ContentPatcher;
using Newtonsoft.Json;
using TehPers.Core.Api.Extensions;

namespace TehPers.FishingOverhaul.Api
{
    public class FishAvailabilityInfo : AvailabilityInfo
    {
        [Description(
            "Effect that sending the bobber by less than the max distance has on the chance. This "
            + "value should be no more than 1."
        )]
        [DefaultValue(0.1d)]
        public double DepthMultiplier { get; set; } = 0.1d;

        [Description("The required fishing depth to maximize the chances of catching the fish.")]
        [DefaultValue(4)]
        public int MaxDepth { get; set; } = 4;

        [JsonConstructor]
        public FishAvailabilityInfo(double baseChance)
            : base(baseChance)
        {
        }

        public override double? GetWeightedChance(
            int fishingLevel,
            IEnumerable<string> locations,
            WaterTypes waterTypes = WaterTypes.All,
            int depth = 4
        )
        {
            return base.GetWeightedChance(fishingLevel, locations, waterTypes, depth)
                .Map(
                    baseChance =>
                        baseChance * (1 - Math.Max(0, this.MaxDepth - depth) * this.DepthMultiplier)
                        + fishingLevel / 50.0f
                );
        }
    }
}
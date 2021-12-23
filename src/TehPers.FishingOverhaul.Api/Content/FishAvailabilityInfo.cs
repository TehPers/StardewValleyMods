using System;
using System.ComponentModel;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Information about the availability of a catchable fish.
    /// </summary>
    /// <inheritdoc cref="AvailabilityInfo"/>
    /// <param name="BaseChance">The base chance of this being caught.</param>
    /// <inheritdoc cref="AvailabilityInfo"/>
    [JsonDescribe]
    public record FishAvailabilityInfo(double BaseChance) : AvailabilityInfo(BaseChance)
    {
        /// <summary>
        /// Effect that sending the bobber by less than the max distance has on the chance. This
        /// value should be no more than 1.
        /// </summary>
        [DefaultValue(0.1d)]
        public double DepthMultiplier { get; init; } = 0.1d;

        /// <summary>
        /// The required fishing depth to maximize the chances of catching the fish.
        /// </summary>
        [DefaultValue(4)]
        public int MaxDepth { get; init; } = 4;

        public override double? GetWeightedChance(FishingInfo fishingInfo)
        {
            return base.GetWeightedChance(fishingInfo)
                .Select(
                    baseChance =>
                        baseChance
                        * (1
                            - Math.Max(0, this.MaxDepth - fishingInfo.BobberDepth) * this.DepthMultiplier)
                        + fishingInfo.FishingLevel / 50.0f
                );
        }
    }
}
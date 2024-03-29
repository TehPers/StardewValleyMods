﻿using StardewValley;
using System;

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for after the chance for a fish is calculated.
    /// </summary>
    public class CalculatedFishChanceEventArgs : ChanceCalculatedEventArgs
    {
        /// <summary>
        /// The chance of hitting a fish (instead of trash).
        /// </summary>
        [Obsolete("Use " + nameof(ChanceCalculatedEventArgs.Chance) + " instead.")]
        public double ChanceForFish
        {
            get => this.Chance;
            set => this.Chance = value;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedFishChanceEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="chanceForFish">The chance of hitting a fish (instead of trash).</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public CalculatedFishChanceEventArgs(FishingInfo fishingInfo, double chanceForFish)
            : base(fishingInfo, chanceForFish)
        {
        }
    }
}

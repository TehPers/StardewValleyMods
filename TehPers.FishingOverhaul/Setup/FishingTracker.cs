using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;

namespace TehPers.FishingOverhaul.Setup
{
    internal class FishingTracker
    {
        public Dictionary<Farmer, ActiveFisher> ActiveFisherData { get; } = new();

        public record ActiveFisher(FishingRod Rod, FishingState State);
    }
}
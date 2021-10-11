using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class FishingTracker
    {
        public Dictionary<Farmer, FisherData> ActiveFisherData { get; } = new();

        public record FisherData(FishingRod Rod, FishingState State);
    }
}
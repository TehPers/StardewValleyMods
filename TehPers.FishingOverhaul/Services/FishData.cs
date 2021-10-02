using System.Collections.Generic;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    public class FishData
    {
        public Dictionary<string, List<FishAvailability>> FishAvailabilities { get; }
        public Dictionary<NamespacedKey, FishTraits> FishTraits { get; }

        public FishData()
        {
            this.FishAvailabilities = new();
            this.FishTraits = new();
        }
    }
}
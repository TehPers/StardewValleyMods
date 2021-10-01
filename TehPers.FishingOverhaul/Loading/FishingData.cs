using System.Collections.Generic;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Loading
{
    public class FishingData
    {
        public Dictionary<string, List<FishAvailability>> FishAvailabilities { get; }
        public Dictionary<NamespacedKey, FishTraits> FishTraits { get; }

        public FishingData()
        {
            this.FishAvailabilities = new();
            this.FishTraits = new();
        }
    }
}
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class FishEntry : Entry<FishAvailabilityInfo>
    {
        [JsonRequired]
        [Description("The item key.")]
        public NamespacedKey FishKey { get; set; }

        [JsonConstructor]
        public FishEntry(NamespacedKey fishKey, FishAvailabilityInfo availabilityInfo)
            : base(availabilityInfo)
        {
            this.FishKey = fishKey;
        }
    }
}
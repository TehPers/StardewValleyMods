using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class FishEntry
    {
        [JsonRequired]
        [Description("The item key.")]
        public NamespacedKey FishKey { get; set; }

        [JsonRequired]
        [Description("The availability information.")]
        public FishAvailability Availability { get; set; }

        [JsonConstructor]
        public FishEntry(NamespacedKey fishKey, FishAvailability availability)
        {
            this.FishKey = fishKey;
            this.Availability = availability;
        }
    }
}
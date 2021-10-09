using System;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class TrashEntry : Entry<AvailabilityInfo>
    {
        [JsonRequired]
        [Description("The item key.")]
        public NamespacedKey ItemKey { get; set; }

        [JsonConstructor]
        public TrashEntry(NamespacedKey itemKey, AvailabilityInfo availabilityInfo)
            : base(availabilityInfo)
        {
            this.ItemKey = itemKey;
            this.AvailabilityInfo = availabilityInfo
                ?? throw new ArgumentNullException(nameof(availabilityInfo));
        }
    }
}
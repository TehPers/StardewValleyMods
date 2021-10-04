using System;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class TrashEntry
    {
        [Description("The item key.")]
        public NamespacedKey ItemKey { get; set; }

        [Description("The availability information.")]
        public Availability Availability { get; set; }

        [JsonConstructor]
        public TrashEntry(
            NamespacedKey itemKey,
            Availability availability
        )
        {
            this.ItemKey = itemKey;
            this.Availability = availability ?? throw new ArgumentNullException(nameof(availability));
        }
    }
}
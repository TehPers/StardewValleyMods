using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api
{
    [JsonDescribe]
    public class TreasureEntry
    {
        [Description("The availability information.")]
        public Availability Availability { get; set; }

        [Description("The possible item keys.")]
        public List<NamespacedKey> ItemKeys { get; set; }

        [Description("The minimum quantity of this item to be added.")]
        public int MinQuantity { get; set; }

        [Description("The maximum quantity of this item to be added.")]
        public int MaxQuantity { get; set; }

        [Description("Whether this can be found multiple times in one chest.")]
        public bool AllowDuplicates { get; set; }

        [JsonConstructor]
        public TreasureEntry(
            Availability availability,
            List<NamespacedKey> itemKeys,
            int minQuantity = 1,
            int maxQuantity = 1,
            bool allowDuplicates = true
        )
        {
            this.ItemKeys = itemKeys ?? throw new ArgumentNullException(nameof(itemKeys));
            this.Availability = availability ?? throw new ArgumentNullException(nameof(availability));
            this.MinQuantity = minQuantity;
            this.MaxQuantity = maxQuantity;
            this.AllowDuplicates = allowDuplicates;
        }
    }
}
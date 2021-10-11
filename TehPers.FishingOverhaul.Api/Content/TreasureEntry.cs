using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Extensions;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// An entry for treasure loot.
    /// </summary>
    /// <inheritdoc cref="Entry{T}"/>
    [JsonDescribe]
    public record TreasureEntry(
        AvailabilityInfo AvailabilityInfo,
        [property: JsonRequired]
        [property:
            Description(
                "The possible namespaced keys for the loot. The item key is chosen randomly."
            )]
        List<NamespacedKey> ItemKeys
    ) : Entry<AvailabilityInfo>(AvailabilityInfo)
    {
        [Description("The minimum quantity of this item. This is only valid for stackable items.")]
        [DefaultValue(1)]
        public int MinQuantity { get; init; } = 1;

        [Description("The maximum quantity of this item. This is only valid for stackable items.")]
        [DefaultValue(1)]
        public int MaxQuantity { get; init; } = 1;

        [Description("Whether this can be found multiple times in one chest.")]
        [DefaultValue(true)]
        public bool AllowDuplicates { get; init; } = true;

        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            var itemKey = this.ItemKeys.ToWeighted(_ => 1).ChooseOrDefault(Game1.random);
            if (itemKey is { Value: var key }
                && namespaceRegistry.TryGetItemFactory(key, out var factory))
            {
                item = new(factory.Create());
                if (item.Item is SObject obj)
                {
                    // Random quantity
                    obj.Stack = Game1.random.Next(this.MinQuantity, this.MaxQuantity);
                }

                return true;
            }

            item = default;
            return false;
        }
    }
}
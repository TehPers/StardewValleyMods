using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Content
{
    internal record LostBookEntry(AvailabilityInfo AvailabilityInfo) : TreasureEntry(
        AvailabilityInfo,
        ImmutableArray.Create(NamespacedKey.SdvObject(102))
    )
    {
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            item = default;
            return false;
        }
    }
}
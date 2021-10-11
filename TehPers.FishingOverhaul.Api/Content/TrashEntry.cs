using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    [JsonDescribe]
    public record TrashEntry(
        [property: JsonRequired] [property: Description("The item key.")] NamespacedKey ItemKey,
        AvailabilityInfo AvailabilityInfo
    ) : Entry<AvailabilityInfo>(AvailabilityInfo)
    {
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            if (namespaceRegistry.TryGetItemFactory(this.ItemKey, out var factory))
            {
                item = new(factory.Create());
                return true;
            }

            item = default;
            return false;
        }
    }
}
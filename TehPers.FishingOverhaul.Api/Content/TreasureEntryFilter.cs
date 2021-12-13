using System.Collections.Immutable;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;
using System.Linq;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Treasure entry filter.
    /// </summary>
    [JsonDescribe]
    public record TreasureEntryFilter
    {
        /// <summary>
        /// The namespaced keys of the treasure.
        /// </summary>
        public ImmutableArray<NamespacedKey>? ItemKeys { get; init; }

        /// <summary>
        /// Checks if the entry matches this filter.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns>Whether the entry matches this filter.</returns>
        public bool Matches(TreasureEntry entry)
        {
            // Check if the item keys match
            if (this.ItemKeys is { } itemKeys && !itemKeys.All(entry.ItemKeys.Contains))
            {
                return false;
            }

            return true;
        }
    }
}
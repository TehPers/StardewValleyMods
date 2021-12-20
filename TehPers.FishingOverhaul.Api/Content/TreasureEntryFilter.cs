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
        /// The namespaced keys of the treasure. This must match every listed item key in the entry
        /// you want to remove. For example, if an entry lists bait, stone, and wood as its
        /// possible item keys, you must list *all* of those to remove it.
        /// </summary>
        public ImmutableArray<NamespacedKey>? ItemKeys { get; init; }

        /// <summary>
        /// A namespaced key in the treasure entry. Any entry that can produce this item will be
        /// removed. This takes precedence over <see cref="ItemKeys"/> (if both are listed and this
        /// condition is matched, then <see cref="ItemKeys"/> is ignored).
        /// </summary>
        public NamespacedKey? AnyWithItem { get; init; }

        /// <summary>
        /// Checks if the entry matches this filter.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns>Whether the entry matches this filter.</returns>
        public bool Matches(TreasureEntry entry)
        {
            // Check if the item keys match
            if (this.AnyWithItem is { } anyWithItem && entry.ItemKeys.Contains(anyWithItem)
                || this.ItemKeys is { } itemKeys && !itemKeys.All(entry.ItemKeys.Contains))
            {
                return false;
            }

            return true;
        }
    }
}
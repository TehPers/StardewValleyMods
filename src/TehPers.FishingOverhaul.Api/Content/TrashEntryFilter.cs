using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Trash entry filter.
    /// </summary>
    [JsonDescribe]
    public record TrashEntryFilter
    {
        /// <summary>
        /// The namespaced key of the trash.
        /// </summary>
        public NamespacedKey? ItemKey { get; init; }

        /// <summary>
        /// Checks if the entry matches this filter.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns>Whether the entry matches this filter.</returns>
        public bool Matches(TrashEntry entry)
        {
            // Check if the item key matches
            if (this.ItemKey is { } itemKey && itemKey == entry.ItemKey)
            {
                return true;
            }

            return false;
        }
    }
}
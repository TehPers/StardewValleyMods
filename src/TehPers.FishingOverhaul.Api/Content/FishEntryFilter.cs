using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Fish entry filter.
    /// </summary>
    public record FishEntryFilter
    {
        /// <summary>
        /// The namespaced key of the fish.
        /// </summary>
        public NamespacedKey? FishKey { get; init; }

        /// <summary>
        /// Checks if the entry matches this filter.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns>Whether the entry matches this filter.</returns>
        public bool Matches(FishEntry entry)
        {
            // Check if the fish key matches
            if (this.FishKey is { } fishKey && fishKey == entry.FishKey)
            {
                return true;
            }

            return false;
        }
    }
}

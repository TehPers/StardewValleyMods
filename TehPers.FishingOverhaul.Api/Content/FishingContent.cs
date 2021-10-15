using System.Collections.Immutable;
using StardewModdingAPI;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Content which affects fishing.
    /// </summary>
    /// <param name="Mod">The manifest of the mod that created this content.</param>
    public record FishingContent(IManifest Mod)
    {
        /// <summary>
        /// Gets the fish traits this content is trying to add.
        /// </summary>
        public ImmutableDictionary<NamespacedKey, FishTraits> FishTraits { get; init; } =
            ImmutableDictionary<NamespacedKey, FishTraits>.Empty;

        /// <summary>
        /// Gets the new fish entries this content wants to create.
        /// </summary>
        public ImmutableArray<FishEntry> FishEntries { get; init; } =
            ImmutableArray<FishEntry>.Empty;

        /// <summary>
        /// Gets the new trash entries this content wants to create.
        /// </summary>
        public ImmutableArray<TrashEntry> TrashEntries { get; init; } =
            ImmutableArray<TrashEntry>.Empty;

        /// <summary>
        /// Gets the new treasure entries this content wants to create.
        /// </summary>
        public ImmutableArray<TreasureEntry> TreasureEntries { get; init; } =
            ImmutableArray<TreasureEntry>.Empty;
    }
}
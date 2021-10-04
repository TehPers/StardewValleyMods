using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Content which affects fishing.
    /// </summary>
    public class FishingContent
    {
        /// <summary>
        /// Gets the manifest of the mod that created this content.
        /// </summary>
        public IManifest Mod { get; }

        /// <summary>
        /// Gets the fish traits this content is trying to add.
        /// </summary>
        public Dictionary<NamespacedKey, FishTraits> FishTraits { get; }

        /// <summary>
        /// Gets the new fish entries this content wants to create.
        /// </summary>
        public List<FishEntry> FishEntries { get; }

        /// <summary>
        /// Gets the new trash entries this content wants to create.
        /// </summary>
        public List<TrashEntry> TrashEntries { get; }

        /// <summary>
        /// Gets the new treasure entries this content wants to create.
        /// </summary>
        public List<TreasureEntry> TreasureEntries { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FishingContent"/> class.
        /// </summary>
        /// <param name="mod">The manifest of the mod that created this content.</param>
        /// <param name="traits">The fish traits this content is trying to add.</param>
        /// <param name="fishEntries">The new fish entries this content wants to create.</param>
        /// <param name="trashEntries">The new trash entries this content wants to create.</param>
        /// <param name="treasureEntries">The new treasure entries this content wants to create.</param>
        public FishingContent(
            IManifest mod,
            Dictionary<NamespacedKey, FishTraits>? traits,
            List<FishEntry>? fishEntries,
            List<TrashEntry>? trashEntries,
            List<TreasureEntry>? treasureEntries
        )
        {
            this.Mod = mod;
            this.FishTraits = traits ?? new();
            this.FishEntries = fishEntries ?? new();
            this.TrashEntries = trashEntries ?? new();
            this.TreasureEntries = treasureEntries ?? new();
        }
    }
}
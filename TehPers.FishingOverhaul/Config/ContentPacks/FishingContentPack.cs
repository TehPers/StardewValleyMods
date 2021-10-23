using System;
using System.Collections.Immutable;
using System.IO;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Fishing content pack.
    /// </summary>
    public record FishingContentPack : JsonConfigRoot
    {
        /// <summary>
        /// Fish traits to add.
        /// </summary>
        public ImmutableDictionary<NamespacedKey, FishTraits> AddFishTraits { get; init; } = ImmutableDictionary<NamespacedKey, FishTraits>.Empty;

        /// <summary>
        /// Fish entries to add.
        /// </summary>
        public ImmutableArray<FishEntry> AddFish { get; init; } = ImmutableArray<FishEntry>.Empty;

        /// <summary>
        /// Trash entries to add.
        /// </summary>
        public ImmutableArray<TrashEntry> AddTrash { get; init; } = ImmutableArray<TrashEntry>.Empty;

        /// <summary>
        /// Treasure entries to add.
        /// </summary>
        public ImmutableArray<TreasureEntry> AddTreasure { get; init; } = ImmutableArray<TreasureEntry>.Empty;

        /// <summary>
        /// The additional content files to include.
        /// </summary>
        public ImmutableArray<string> Include { get; init; } = ImmutableArray<string>.Empty;

        /// <summary>
        /// Merges all the content into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        /// <param name="baseDir">The base directory to load included content from.</param>
        /// <param name="contentPack">The content pack.</param>
        /// <param name="jsonProvider">The JSON provider.</param>
        /// <param name="monitor">The monitor to log errors to.</param>
        public FishingContent AddTo(FishingContent content, string baseDir, IContentPack contentPack, IJsonProvider jsonProvider, IMonitor monitor)
        {
            // Add base content
            content = content with
            {
                FishTraits = content.FishTraits.AddRange(this.AddFishTraits),
                FishEntries = content.FishEntries.AddRange(this.AddFish),
                TrashEntries = content.TrashEntries.AddRange(this.AddTrash),
                TreasureEntries = content.TreasureEntries.AddRange(this.AddTreasure),
            };

            // Add included content
            foreach (string relativePath in this.Include)
            {
                // Get the full path to the included file
                var path = Path.Combine(baseDir, relativePath);

                // Load the included file
                FishingContentPack? included;
                try
                {
                    included = jsonProvider.ReadJson<FishingContentPack>(path, new ContentPackAssetProvider(contentPack), null);
                }
                catch (Exception ex)
                {
                    monitor.Log($"Failed to load included content pack '{path}'", LogLevel.Error);
                    monitor.Log(ex.ToString(), LogLevel.Error);
                    continue;
                }

                // Merge the included content
                if (included is not null)
                {
                    var contentBaseDir = Path.GetDirectoryName(path) ?? string.Empty;
                    content = included.AddTo(content, contentBaseDir, contentPack, jsonProvider, monitor);
                }
                else
                {
                    monitor.Log($"Content file is empty: {relativePath}", LogLevel.Error);
                }
            }

            return content;
        }
    }
}
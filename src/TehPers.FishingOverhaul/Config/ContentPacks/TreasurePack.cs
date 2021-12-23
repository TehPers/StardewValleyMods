using System.Collections.Immutable;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what treasure are available to catch.
    /// </summary>
    [JsonDescribe]
    public record TreasurePack : JsonConfigRoot
    {
        /// <summary>
        /// The treasure entries to add.
        /// </summary>
        public ImmutableArray<TreasureEntry> Add { get; init; } = ImmutableArray<TreasureEntry>.Empty;

        /// <summary>
        /// Merges all the treasure entries into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with { AddTreasure = content.AddTreasure.AddRange(this.Add) };
        }
    }
}
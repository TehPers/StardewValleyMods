using System.Collections.Immutable;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what trash are available to catch.
    /// </summary>
    [JsonDescribe]
    public record TrashPack : JsonConfigRoot
    {
        /// <summary>
        /// The trash entries to add.
        /// </summary>
        public ImmutableArray<TrashEntry> Add { get; init; } = ImmutableArray<TrashEntry>.Empty;

        /// <summary>
        /// Merges all the trash entries into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with { TrashEntries = content.TrashEntries.AddRange(this.Add) };
        }
    }
}
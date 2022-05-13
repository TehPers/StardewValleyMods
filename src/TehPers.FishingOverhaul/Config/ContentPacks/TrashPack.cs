using System.Collections.Immutable;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what trash are available to catch.
    /// </summary>
    public record TrashPack : JsonConfigRoot
    {
        /// <inheritdoc cref="JsonConfigRoot.Schema" />
        protected override string Schema =>
            $"{JsonConfigRoot.jsonSchemaRootUrl}contentPacks/trash.schema.json";

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
            return content with {AddTrash = content.AddTrash.AddRange(this.Add)};
        }
    }
}

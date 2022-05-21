using System.Collections.Immutable;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what fish are available to catch.
    /// </summary>
    public record FishPack : JsonConfigRoot
    {
        /// <inheritdoc cref="JsonConfigRoot.Schema" />
        protected override string Schema =>
            $"{JsonConfigRoot.jsonSchemaRootUrl}contentPacks/fish.schema.json";

        /// <summary>
        /// The fish entries to add.
        /// </summary>
        public ImmutableArray<FishEntry> Add { get; init; } = ImmutableArray<FishEntry>.Empty;

        /// <summary>
        /// Merges all the fish entries into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with {AddFish = content.AddFish.AddRange(this.Add)};
        }
    }
}

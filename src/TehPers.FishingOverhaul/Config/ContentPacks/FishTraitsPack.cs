using System.Collections.Immutable;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which modifies the behavior of fish.
    /// </summary>
    [JsonDescribe]
    public record FishTraitsPack : JsonConfigRoot
    {
        /// <inheritdoc cref="JsonConfigRoot.Schema" />
        protected override string Schema =>
            $"{JsonConfigRoot.jsonSchemaRootUrl}contentPacks/fishTraits.schema.json";

        /// <summary>
        /// The fish traits to add.
        /// </summary>
        public ImmutableDictionary<NamespacedKey, FishTraits> Add { get; init; } =
            ImmutableDictionary<NamespacedKey, FishTraits>.Empty;

        /// <summary>
        /// Merges all the traits into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with {SetFishTraits = content.SetFishTraits.AddRange(this.Add)};
        }
    }
}

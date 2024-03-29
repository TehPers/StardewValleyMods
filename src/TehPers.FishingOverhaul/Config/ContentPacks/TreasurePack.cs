﻿using System.Collections.Immutable;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what treasure are available to catch.
    /// </summary>
    public record TreasurePack : JsonConfigRoot
    {
        /// <inheritdoc cref="JsonConfigRoot.Schema" />
        protected override string Schema =>
            $"{JsonConfigRoot.jsonSchemaRootUrl}contentPacks/treasure.schema.json";

        /// <summary>
        /// The treasure entries to add.
        /// </summary>
        public ImmutableArray<TreasureEntry> Add { get; init; } =
            ImmutableArray<TreasureEntry>.Empty;

        /// <summary>
        /// Merges all the treasure entries into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with {AddTreasure = content.AddTreasure.AddRange(this.Add)};
        }
    }
}

using System.Collections.Immutable;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what treasure are available to catch.
    /// </summary>
    /// <param name="Add">The treasure entries to add.</param>
    [JsonDescribe]
    public record TreasurePack(
        [property: JsonRequired] ImmutableArray<TreasureEntry> Add
    ) : JsonConfigRoot;
}
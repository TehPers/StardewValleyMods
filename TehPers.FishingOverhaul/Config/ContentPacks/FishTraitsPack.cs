using System.Collections.Immutable;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which modifies the behavior of fish.
    /// </summary>
    /// <param name="Add">The fish traits to add.</param>
    [JsonDescribe]
    public record FishTraitsPack(
        [property: JsonRequired] ImmutableDictionary<NamespacedKey, FishTraits> Add
    ) : JsonConfigRoot;
}
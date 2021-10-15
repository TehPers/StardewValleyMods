using System.Collections.Immutable;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what fish are available to catch.
    /// </summary>
    /// <param name="Add">The fish entries to add.</param>
    [JsonDescribe]
    public record FishPack([property: JsonRequired] ImmutableArray<FishEntry> Add) : JsonConfigRoot;
}
using System.Collections.Generic;
using Newtonsoft.Json;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    public class FishTraitsPack : JsonConfigRoot
    {
        [JsonRequired]
        public Dictionary<string, FishTraits> FishTraits { get; init; } = new();
    }
}
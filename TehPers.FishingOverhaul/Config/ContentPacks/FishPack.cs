using System.Collections.Generic;
using Newtonsoft.Json;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    public class FishPack : JsonConfigRoot
    {
        [JsonRequired]
        public List<FishEntry> FishEntries { get; init; } = new();
    }
}
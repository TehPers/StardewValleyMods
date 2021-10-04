using System.Collections.Generic;
using Newtonsoft.Json;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    public class TreasurePack : JsonConfigRoot
    {
        [JsonRequired]
        public List<TreasureEntry> TreasureEntries { get; init; } = new();
    }
}
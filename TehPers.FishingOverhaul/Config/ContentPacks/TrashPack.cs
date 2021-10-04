using System.Collections.Generic;
using Newtonsoft.Json;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    public class TrashPack : JsonConfigRoot
    {
        [JsonRequired]
        public List<TrashEntry> TrashEntries { get; init; } = new();
    }
}
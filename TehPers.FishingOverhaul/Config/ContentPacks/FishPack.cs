using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    [JsonDescribe]
    public class FishPack : JsonConfigRoot
    {
        [JsonRequired]
        [Description("The fish entries to add.")]
        public List<FishEntry> FishEntries { get; init; } = new();
    }
}
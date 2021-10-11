using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    [JsonDescribe]
    public class TreasurePack : JsonConfigRoot
    {
        [JsonRequired]
        [Description("The treasure entries to add.")]
        public List<TreasureEntry> TreasureEntries { get; init; } = new();
    }
}
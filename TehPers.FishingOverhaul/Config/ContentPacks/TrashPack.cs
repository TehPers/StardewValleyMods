using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    [JsonDescribe]
    public class TrashPack : JsonConfigRoot
    {
        [JsonRequired]
        [Description("The trash entries to add.")]
        public List<TrashEntry> Add { get; init; } = new();
    }
}
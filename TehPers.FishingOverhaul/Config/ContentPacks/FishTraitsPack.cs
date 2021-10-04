using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    [JsonDescribe]
    public class FishTraitsPack : JsonConfigRoot
    {
        [JsonRequired]
        [Description("The fish traits to add.")]
        public Dictionary<string, FishTraits> FishTraits { get; init; } = new();
    }
}
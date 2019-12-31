using System.Collections.Generic;
using System.ComponentModel;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    public class FishEntriesConfiguration
    {
        [Description("The fish that can be caught while fishing.")]
        public List<FishAvailability> PossibleFish { get; private set; }
    }
}
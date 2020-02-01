using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Private setters are called by the JSON deserializer through reflection.")]
    public class FishEntriesConfiguration
    {
        [Description("The fish that can be caught while fishing.")]
        public List<FishAvailability> PossibleFish { get; private set; } = new List<FishAvailability>();
    }
}
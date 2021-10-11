using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    [JsonDescribe]
    public record FishTraits(
        [property: JsonRequired]
        [property: Description("How often the fish darts in the fishing minigame.")]
        int DartFrequency,
        [property: JsonRequired]
        [property: Description("How the fish darts during the fishing minigame.")]
        DartBehavior DartBehavior,
        [property: JsonRequired] [property: Description("The minimum size the fish can be.")]
        int MinSize,
        [property: JsonRequired] [property: Description("The maximum size the fish can be.")]
        int MaxSize
    )
    {
        [Description("Whether the fish is legendary.")]
        [DefaultValue(false)]
        public bool IsLegendary { get; set; } = false;
    }
}
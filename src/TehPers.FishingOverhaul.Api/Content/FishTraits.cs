using System.ComponentModel;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Definition for a fish's traits.
    /// </summary>
    /// <param name="DartFrequency">How often the fish darts in the fishing minigame.</param>
    /// <param name="DartBehavior">How the fish moves during the fishing minigame.</param>
    /// <param name="MinSize">The minimum size the fish can be.</param>
    /// <param name="MaxSize">The maximum size the fish can be.</param>
    [JsonDescribe]
    public record FishTraits(
        [property: JsonRequired] int DartFrequency,
        [property: JsonRequired] DartBehavior DartBehavior,
        [property: JsonRequired] int MinSize,
        [property: JsonRequired] int MaxSize
    )
    {
        /// <summary>
        /// Whether the fish is legendary.
        /// </summary>
        [DefaultValue(false)]
        public bool IsLegendary { get; init; } = false;
    }
}
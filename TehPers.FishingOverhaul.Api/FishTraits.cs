using System.ComponentModel;

namespace TehPers.FishingOverhaul.Api
{
    public record FishTraits(
        [property: Description("How often the fish darts in the fishing minigame.")]
        int DartFrequency,
        [property: Description("How the fish darts during the fishing minigame.")]
        string DartBehavior,
        [property: Description("The minimum size the fish can be.")]
        int MinSize,
        [property: Description("The maximum size the fish can be.")]
        int MaxSize,
        [property: Description("Whether the fish is legendary")]
        bool IsLegendary = false
    );
}
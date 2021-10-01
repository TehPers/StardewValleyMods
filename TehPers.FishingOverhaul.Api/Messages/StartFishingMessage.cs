using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Messages
{
    /// <summary>
    /// A <see cref="StardewValley.Farmer"/> has begun the fishing minigame.
    /// </summary>
    public record StartFishingMessage(long FarmerId, NamespacedKey FishKey);
}
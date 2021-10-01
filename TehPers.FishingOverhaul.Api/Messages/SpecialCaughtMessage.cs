using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Messages
{
    /// <summary>
    /// A special item was caught by a <see cref="StardewValley.Farmer"/>.
    /// </summary>
    public record SpecialCaughtMessage(long FarmerId, NamespacedKey ItemKey);
}
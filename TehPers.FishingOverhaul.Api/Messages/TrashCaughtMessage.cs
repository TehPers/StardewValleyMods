using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Messages
{
    /// <summary>
    /// Trash was caught by a <see cref="StardewValley.Farmer"/>.
    /// </summary>
    public record TrashCaughtMessage(long FarmerId, NamespacedKey ItemKey);
}
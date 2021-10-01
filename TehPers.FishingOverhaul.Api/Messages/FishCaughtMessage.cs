using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Messages
{
    /// <summary>
    /// A <see cref="StardewValley.Farmer"/> has caught a fish.
    /// </summary>
    public record FishCaughtMessage(
        long FarmerId,
        NamespacedKey FishKey,
        int FishSize,
        int FishQuality,
        int FishDifficulty,
        bool WasTreasureCaught,
        bool WasPerfectCatch,
        bool FromFishPond,
        bool CaughtDouble = false
    );
}
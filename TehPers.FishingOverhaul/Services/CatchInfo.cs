using StardewValley;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Services
{
    public abstract record CatchInfo(NamespacedKey ItemKey, Item Item, bool FromFishPond)
    {
        public sealed record FishCatch(
            NamespacedKey ItemKey,
            Item Item,
            int FishSize,
            bool IsLegendary,
            int FishQuality,
            int FishDifficulty,
            bool WasTreasureCaught,
            bool WasPerfectCatch,
            bool FromFishPond,
            bool CaughtDouble = false
        ) : CatchInfo(ItemKey, Item, FromFishPond);

        public sealed record TrashCatch(NamespacedKey ItemKey, Item Item, bool FromFishPond) : CatchInfo(
            ItemKey,
            Item,
            FromFishPond
        );

        public sealed record SpecialCatch(NamespacedKey ItemKey, Item Item, bool FromFishPond) : CatchInfo(
            ItemKey,
            Item,
            FromFishPond
        );
    }
}
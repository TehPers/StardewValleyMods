using StardewValley;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Api
{
    public abstract record CatchInfo(FishingInfo FishingInfo, Item FishItem, bool FromFishPond)
    {
        public sealed record FishCatch(
            FishingInfo FishingInfo,
            FishEntry FishEntry,
            Item Item,
            int FishSize,
            bool IsLegendary,
            int FishQuality,
            int FishDifficulty,
            MinigameState State,
            bool FromFishPond,
            bool CaughtDouble = false
        ) : CatchInfo(FishingInfo, Item, FromFishPond);

        public sealed record TrashCatch(
            FishingInfo FishingInfo,
            TrashEntry TrashEntry,
            Item TrashItem,
            bool FromFishPond
        ) : CatchInfo(FishingInfo, TrashItem, FromFishPond);
    }
}
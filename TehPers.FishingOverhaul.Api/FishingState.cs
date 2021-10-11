using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api
{
    public abstract record FishingState
    {
        public sealed record NotFishing : FishingState;

        public sealed record WaitingForBite(FishingInfo FishingInfo) : FishingState;

        public sealed record Fishing(FishingInfo FishingInfo, NamespacedKey FishKey) : FishingState;

        public sealed record Caught(FishingInfo FishingInfo, CatchInfo Catch) : FishingState;

        public sealed record Holding(FishingInfo FishingInfo, CatchInfo Catch) : FishingState;

        public sealed record OpeningTreasure : FishingState;

        public static FishingState Start()
        {
            return new NotFishing();
        }
    }
}
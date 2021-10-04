using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Services.Data
{
    internal abstract record FishingState
    {
        public sealed record NotFishing : FishingState;

        public sealed record WaitingForBite : FishingState;

        public sealed record Fishing(NamespacedKey FishKey) : FishingState;

        public sealed record Caught(CatchInfo Catch) : FishingState;

        public sealed record Holding(CatchInfo Catch) : FishingState;

        public sealed record OpeningTreasure : FishingState;

        public static FishingState Start()
        {
            return new NotFishing();
        }
    }
}
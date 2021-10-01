using StardewValley;
using StardewValley.Menus;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Setup
{
    internal abstract record FishingState
    {
        public sealed record NotFishing : FishingState;

        public sealed record Fishing : FishingState;

        public sealed record CaughtFish(CatchInfo Fish) : FishingState;

        public sealed record HoldingFish(CatchInfo Fish) : FishingState;

        public sealed record DoneFishing : FishingState;

        public sealed record CatchInfo(
            NamespacedKey FishKey,
            Item Fish,
            int FishSize,
            bool IsLegendary
        );

        public static FishingState Start()
        {
            return new NotFishing();
        }

        public FishingState StartFishing()
        {
            return this switch
            {
                NotFishing => new Fishing(),
                _ => this,
            };
        }

        public FishingState CatchFish(CatchInfo fish)
        {
            return this switch
            {
                Fishing => new CaughtFish(fish),
                _ => this,
            };
        }

        public FishingState HoldCaughtItem()
        {
            return this switch
            {
                CaughtFish (var fish) => new HoldingFish(fish),
                _ => this
            };
        }
    }
}
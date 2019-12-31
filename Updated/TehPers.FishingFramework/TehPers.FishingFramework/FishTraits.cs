using System;
using TehPers.Core.Api;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Minigame;

namespace TehPers.FishingFramework
{
    public class FishTraits : IFishTraits
    {
        public NamespacedId ItemId { get; }
        public bool IsLegendary { get; }
        public float BaseDifficulty { get; }
        public float MinSize { get; }
        public float MaxSize { get; }
        public ICatchableEntityController<BobberBarCatchableItem<NamespacedId>> Controller { get; }

        public FishTraits(NamespacedId itemId, float baseDifficulty, float minSize, float maxSize, ICatchableEntityController<BobberBarCatchableItem<NamespacedId>> controller, bool isLegendary = false)
        {
            this.ItemId = itemId;
            this.IsLegendary = isLegendary;
            this.BaseDifficulty = baseDifficulty;
            this.MinSize = minSize;
            this.MaxSize = maxSize;
            this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }
    }
}
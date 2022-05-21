using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Effects;

namespace TehPers.FishingOverhaul.Effects
{
    internal class EmptyEffect : IFishingEffect
    {
        public void Apply(FishingInfo fishingInfo)
        {
        }

        public void Unapply(FishingInfo fishingInfo)
        {
        }

        public void UnapplyAll()
        {
        }
    }
}

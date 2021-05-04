using StardewValley;
using TehPers.Core.Api.Weighted;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class WeightedFishData : IWeighted {
        public int Fish { get; }
        public IFishData Data { get; }
        public Farmer Who { get; }

        public WeightedFishData(int fish, IFishData data, Farmer who) {
            Fish = fish;
            Data = data;
            Who = who;
        }

        public double GetWeight() {
            return Data.GetWeight(Who);
        }

    }
}

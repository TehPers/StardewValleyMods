using FishingOverhaul.Api;
using TehCore.Api.Weighted;

namespace FishingOverhaul.Configs {
    public class WeightedFishData : IWeighted {
        public int Fish { get; }
        public IFishData Data { get; }
        public int Level { get; }

        public WeightedFishData(int fish, IFishData data, int level) {
            this.Fish = fish;
            this.Data = data;
            this.Level = level;
        }

        public double GetWeight() {
            return this.Data.GetWeight(this.Level);
        }

    }
}

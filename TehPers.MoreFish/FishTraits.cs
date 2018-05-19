using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Enums;

namespace TehPers.MoreFish {
    public class FishTraits : IFishTraits {
        public string Name { get; }
        public int Price { get; }
        public int Edibility { get; }
        public float Difficulty { get; }
        public int MaxSize { get; }
        public int MinSize { get; }
        public FishMotionType MotionType { get; }

        public FishTraits(string name, float difficulty, int minSize, int maxSize, FishMotionType motionType, int price, int edibility = -300) {
            this.Name = name;
            this.Difficulty = difficulty;
            this.MinSize = minSize;
            this.MaxSize = maxSize;
            this.MotionType = motionType;
            this.Price = price;
            this.Edibility = edibility;
        }
    }
}
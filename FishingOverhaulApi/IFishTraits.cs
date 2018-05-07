namespace FishingOverhaul.Api {
    public interface IFishTraits {
        float Difficulty { get; }
        int MaxSize { get; }
        int MinSize { get; }
        FishMotionType MotionType { get; }
    }
}
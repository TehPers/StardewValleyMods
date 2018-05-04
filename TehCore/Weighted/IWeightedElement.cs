namespace TehCore.Weighted {
    public interface IWeightedElement<out T> : IWeighted {
        T Value { get; }
    }
}
namespace TehCore.Api.Weighted {
    public interface IWeightedElement<out T> : IWeighted {
        /// <summary>The value wrapped by this element.</summary>
        T Value { get; }
    }
}
namespace TehPers.FishingOverhaul.Api.Weighted
{
    public class WeightedValue<T> : IWeightedValue<T>
    {
        public T Value { get; }

        public double Weight { get; }

        /// <summary>Initializes a new instance of the <see cref="WeightedValue{T}"/> class.</summary>
        /// <param name="value">The value of this <see cref="WeightedValue{T}"/>.</param>
        /// <param name="weight">The weighted chance for this <see cref="WeightedValue{T}"/>.</param>
        public WeightedValue(T value, double weight)
        {
            this.Value = value;
            this.Weight = weight;
        }
    }
}

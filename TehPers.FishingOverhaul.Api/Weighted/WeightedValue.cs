namespace TehPers.FishingOverhaul.Api.Weighted
{
    /// <summary>
    /// A value and a weight.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public class WeightedValue<T> : IWeightedValue<T>
    {
        /// <summary>
        /// The inner value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The weighted chance of this value.
        /// </summary>
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

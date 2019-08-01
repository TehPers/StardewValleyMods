namespace TehPers.Core.Api.Weighted
{
    public class WeightedValue<T> : IWeightedValue<T>
    {
        public T Value { get; }
        public double Weight { get; }

        public WeightedValue(T value, double weight)
        {
            this.Value = value;
            this.Weight = weight;
        }
    }
}

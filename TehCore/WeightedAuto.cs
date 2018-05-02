namespace TehCore {
    public static partial class Extensions {
        private class WeightedAuto<T> : IWeighted {
            public readonly T Element;
            private readonly double _weight;

            public WeightedAuto(T elem, double weight) {
                this.Element = elem;
                this._weight = weight;
            }

            public double GetWeight() {
                return this._weight;
            }
        }
    }
}

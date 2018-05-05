using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehCore.Weighted;

namespace TehCore.Helpers {
    public static class WeightedHelpers {
        public static void Shuffle<T>(this IList<T> source) => source.Shuffle(new Random());
        public static void Shuffle<T>(this IList<T> source, Random rand) {
            int n = source.Count;
            while (n > 1) {
                n--;
                int k = rand.Next(n + 1);
                T value = source[k];
                source[k] = source[n];
                source[n] = value;
            }
        }

        public static T Choose<T>(this IEnumerable<KeyValuePair<T, double>> source) => source.Choose(new Random());
        public static T Choose<T>(this IEnumerable<KeyValuePair<T, double>> source, Random rand) {
            return source.Select(kv => new WeightedElement<T>(kv.Key, kv.Value)).Choose<T>(rand);
        }

        public static T Choose<T>(this IEnumerable<T> source) where T : IWeighted => source.Choose(new Random());
        public static T Choose<T>(this IEnumerable<T> source, Random rand) where T : IWeighted {
            source = source.ToList();
            double totalWeight = source.Sum(entry => entry.GetWeight());
            double n = rand.NextDouble();
            foreach (T entry in source) {
                double chance = entry.GetWeight() / totalWeight;
                if (n < chance) return entry;
                else n -= chance;
            }
            throw new ArgumentException("Enumerable must contain entries", nameof(source));
        }

        public static T Choose<T>(this IEnumerable<IWeightedElement<T>> source) => source.Choose(new Random());
        public static T Choose<T>(this IEnumerable<IWeightedElement<T>> source, Random rand) {
            IWeighted result = ((IEnumerable<IWeighted>) source).Choose(rand);
            return ((WeightedElement<T>) result).Value;
        }

        public static IEnumerable<IWeightedElement<T>> ToWeighted<T>(this IEnumerable<T> source, Func<T, double> weightSelector) => source.ToWeighted(weightSelector, e => e);
        public static IEnumerable<IWeightedElement<TEntry>> ToWeighted<TSource, TEntry>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector, Func<TSource, TEntry> entrySelector) {
            return source.Select(e => new WeightedElement<TEntry>(entrySelector(e), weightSelector(e)));
        }

        public static IEnumerable<IWeightedElement<T>> Normalize<T>(this IEnumerable<T> source) where T : IWeighted => source.NormalizeTo(1D);
        public static IEnumerable<IWeightedElement<T>> NormalizeTo<T>(this IEnumerable<T> source, double weight) where T : IWeighted {
            source = source.ToList();
            double totalWeight = source.SumWeights();
            if (totalWeight == 0)
                totalWeight = 1;
            return source.Select(e => new WeightedElement<T>(e, weight * e.GetWeight() / totalWeight));
        }

        public static IEnumerable<IWeightedElement<T>> Normalize<T>(this IEnumerable<IWeightedElement<T>> source) => source.NormalizeTo(1D);
        public static IEnumerable<IWeightedElement<T>> NormalizeTo<T>(this IEnumerable<IWeightedElement<T>> source, double weight) {
            source = source.ToList();
            double totalWeight = source.SumWeights();
            if (totalWeight == 0)
                totalWeight = 1;
            return source.Select(e => new WeightedElement<T>(e.Value, weight * e.GetWeight() / totalWeight));
        }

        public static double SumWeights<T>(this IEnumerable<T> source) where T : IWeighted => source.Sum(e => e.GetWeight());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api.Weighted;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>Extension methods for <see cref="IEnumerable{T}"/> for weighted random item selection.</summary>
    public static class WeightedExtensions
    {
        /// <summary>Chooses a random item using the items' weights, or a default value if there are no items.</summary>
        /// <typeparam name="T">The type of item in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="rand">The <see cref="Random"/> to use.</param>
        /// <returns>The chosen item.</returns>
        public static T ChooseOrDefault<T>(this IEnumerable<T> source, Random rand)
            where T : IWeighted
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            var enumeratedSource = source as T[] ?? source.ToArray();
            return enumeratedSource.Any() ? enumeratedSource.Choose(rand) : default;
        }

        /// <summary>Chooses a random item using the items' weights.</summary>
        /// <typeparam name="T">The type of item in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="rand">The <see cref="Random"/> to use.</param>
        /// <returns>The chosen item.</returns>
        public static T Choose<T>(this IEnumerable<T> source, Random rand)
            where T : IWeighted
        {
            _ = rand ?? throw new ArgumentNullException(nameof(rand));
            _ = source ?? throw new ArgumentNullException(nameof(source));

            var enumeratedSource = source as T[] ?? source.ToArray();
            if (!enumeratedSource.Any())
            {
                throw new ArgumentException("Source must contain entries", nameof(source));
            }

            var totalWeight = enumeratedSource.SumWeights();
            if (Math.Abs(totalWeight) < double.Epsilon * 10)
            {
                throw new ArgumentException("Source must have a non-zero total weight", nameof(source));
            }

            var n = rand.NextDouble();
            foreach (var entry in enumeratedSource)
            {
                var chance = entry.Weight / totalWeight;
                if (n < chance)
                {
                    return entry;
                }

                n -= chance;
            }

            throw new ArgumentException("Source should contain positively weighted entries", nameof(source));
        }

        /// <summary>Converts the items in an <see cref="IEnumerable{T}"/> to <see cref="IWeightedValue{T}"/>.</summary>
        /// <typeparam name="T">The type of item in the source <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="weightSelector">A callback which maps each item to its weight.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all the items converted to <see cref="IWeightedValue{T}"/>.</returns>
        public static IEnumerable<IWeightedValue<T>> ToWeighted<T>(this IEnumerable<T> source, Func<T, double> weightSelector)
        {
            return source.ToWeighted(weightSelector, e => e);
        }

        /// <summary>Converts the items in an <see cref="IEnumerable{T}"/> to <see cref="IWeightedValue{T}"/>.</summary>
        /// <typeparam name="TSource">The type of item in the source <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TEntry">The type of values in the resulting <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="weightSelector">A callback which maps each item to its weight.</param>
        /// <param name="elementSelector">A callback which maps each item to its value.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all the items converted to <see cref="IWeightedValue{T}"/>.</returns>
        public static IEnumerable<IWeightedValue<TEntry>> ToWeighted<TSource, TEntry>(this IEnumerable<TSource> source, Func<TSource, double> weightSelector, Func<TSource, TEntry> elementSelector)
        {
            return source.Select(e => new WeightedValue<TEntry>(elementSelector(e), weightSelector(e))).ToArray();
        }

        /// <summary>Normalizes the weights of each item and returns a new <see cref="IEnumerable{T}"/> with the normalized items.</summary>
        /// <typeparam name="T">The type of item in the <see cref="IEnumerable{T}"/>. It must implement <see cref="IWeighted"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="weight">The weight to normalize to.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing new, normalized items in the same order.</returns>
        public static IEnumerable<IWeightedValue<T>> Normalize<T>(this IEnumerable<T> source, double weight = 1D)
            where T : IWeighted
        {
            var enumeratedSource = source as T[] ?? source.ToArray();
            var totalWeight = enumeratedSource.SumWeights();
            if (Math.Abs(totalWeight) < double.Epsilon)
            {
                throw new ArgumentException("The weights of all of the source items sum to zero.", nameof(source));
            }

            return enumeratedSource.Select(e => new WeightedValue<T>(e, weight * e.Weight / totalWeight)).ToArray();
        }

        /// <summary>Normalizes the weights of each <see cref="IWeightedValue{T}"/> and returns a new <see cref="IEnumerable{T}"/> with the normalized items.</summary>
        /// <typeparam name="T">The type of value in the <see cref="IWeightedValue{T}"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="weight">The weight to normalize to.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing new, normalized items in the same order.</returns>
        public static IEnumerable<IWeightedValue<T>> Normalize<T>(this IEnumerable<IWeightedValue<T>> source, int weight = 1)
        {
            var enumeratedSource = source as IWeightedValue<T>[] ?? source.ToArray();
            var totalWeight = enumeratedSource.SumWeights();
            if (Math.Abs(totalWeight) < double.Epsilon)
            {
                throw new ArgumentException("The weights of all of the source items sum to zero.", nameof(source));
            }

            return enumeratedSource.Select(e => new WeightedValue<T>(e.Value, weight * e.Weight / totalWeight));
        }

        /// <summary>Sums all of the weights of the items in the <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="T">The type of item in the <see cref="IEnumerable{T}"/>. It must implement <see cref="IWeighted"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <returns>The sum of the weights.</returns>
        public static double SumWeights<T>(this IEnumerable<T> source)
            where T : IWeighted
        {
            return source.Sum(e => e.Weight);
        }
    }
}
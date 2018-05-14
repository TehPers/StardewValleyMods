﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TehPers.Core.Helpers {
    public static class EnumerableHelpers {
        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <see cref="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => new HashSet<TSource>(source.ToArray());

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) => source.ToDictionary(kv => kv.Key, kv => kv.Value);

        /// <summary>Retrieves a value from a <see cref="IDictionary{TKey,TValue}"/> with the given fallback value</summary>
        /// <typeparam name="TKey">The <see cref="IDictionary{TKey,TValue}"/>'s key type</typeparam>
        /// <typeparam name="TVal">The <see cref="IDictionary{TKey,TValue}"/>'s value type</typeparam>
        /// <param name="source">The dictionary to try to retrieve the value from</param>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="fallback">The fallback value if the key doesn't exist in the dictionary</param>
        /// <returns>If the key exists in <see cref="source"/>, the value associated with <see cref="key"/>, otherwise <see cref="fallback"/></returns>
        public static TVal GetDefault<TKey, TVal>(this IDictionary<TKey, TVal> source, TKey key, TVal fallback = default(TVal)) => source.ContainsKey(key) ? source[key] : fallback;

        /// <summary>Splits an <see cref="IEnumerable{T}"/> into several smaller ones, each containing at most a certain number of elements.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to split.</param>
        /// <param name="size">The number of elements each group should have. The last group may contain fewer elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the elements from <see cref="source"/> split into groups of at most <see cref="source"/> elements.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size) {
            bool grouping;
            using (IEnumerator<T> enumerator = source.GetEnumerator()) {
                grouping = enumerator.MoveNext();
                while (grouping) {
                    yield return GetGroup(enumerator);
                }
            }

            IEnumerable<T> GetGroup(IEnumerator<T> e) {
                int n = size;
                while (n-- > 0 && grouping) {
                    yield return e.Current;
                    grouping = e.MoveNext();
                }
            }
        }

        /// <summary>Creates a new string containing one string repeated any number of times.</summary>
        /// <param name="input">The string to repeat</param>
        /// <param name="count">How many times to repeat it</param>
        /// <returns><see cref="input"/> repeated <see cref="count"/> times</returns>
        /// <remarks>Based on this SO answer: https://stackoverflow.com/a/3754626/8430206 </remarks>
        public static string Repeat(this string input, int count) {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            StringBuilder builder = new StringBuilder(input.Length * count);
            for (int i = 0; i < count; i++) {
                builder.Append(input);
            }

            return builder.ToString();

        }
    }
}

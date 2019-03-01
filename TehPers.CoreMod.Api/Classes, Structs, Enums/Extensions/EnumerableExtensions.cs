using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TehPers.CoreMod.Api.Extensions {
    public static class EnumerableExtensions {
        /// <summary>Shuffles a list.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to shuffle.</param>
        public static void Shuffle<T>(this IList<T> source) {
            source.Shuffle(new Random());
        }

        /// <summary>Shuffles a list.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to shuffle.</param>
        /// <param name="rand">The <see cref="Random"/> to use while shuffling.</param>
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

        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <see cref="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed.</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) {
            return source.ToHashSet(EqualityComparer<TSource>.Default);
        }

        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <param name="comparer">The comparer for the hash set.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <see cref="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed.</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer) {
            return new HashSet<TSource>(source.ToArray(), comparer);
        }

        /// <summary>Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="Dictionary{TKey,TValue}"/>.</summary>
        /// <typeparam name="TKey">The type of the keys in the <paramref name="source" />.</typeparam>
        /// <typeparam name="TValue">The type of the values in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <returns>A dictionary containing all the <see cref="KeyValuePair{TKey,TValue}"/> entries in the <paramref name="source"/>.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) {
            return source.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="Dictionary{TKey,TValue}"/> using a custom <see cref="IEqualityComparer{T}"/>.</summary>
        /// <typeparam name="TKey">The type of the keys in the <paramref name="source" />.</typeparam>
        /// <typeparam name="TValue">The type of the values in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="comparer">The comparer used to compare keys in the dictionary.</param>
        /// <returns>A dictionary containing all the <see cref="KeyValuePair{TKey,TValue}"/> entries in the <paramref name="source"/>.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer) {
            return source.ToDictionary(kv => kv.Key, kv => kv.Value, comparer);
        }

        /// <summary>Swaps two elements in a list.</summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="first">The index of the first element.</param>
        /// <param name="second">The index of the second element.</param>
        public static void Swap<T>(this IList<T> source, int first, int second) {
            T tmp = source[first];
            source[first] = source[second];
            source[second] = tmp;
        }

        /// <summary>Retrieves a value from a <see cref="IDictionary{TKey,TValue}"/> with the given fallback value</summary>
        /// <typeparam name="TKey">The <see cref="IDictionary{TKey,TValue}"/>'s key type</typeparam>
        /// <typeparam name="TVal">The <see cref="IDictionary{TKey,TValue}"/>'s value type</typeparam>
        /// <param name="source">The dictionary to try to retrieve the value from</param>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="fallback">The fallback value if the key doesn't exist in the dictionary</param>
        /// <returns>If the key exists in <see cref="source"/>, the value associated with <see cref="key"/>, otherwise <see cref="fallback"/></returns>
        public static TVal GetDefault<TKey, TVal>(this IDictionary<TKey, TVal> source, TKey key, TVal fallback = default) {
            return source.TryGetValue(key, out TVal value) ? value : fallback;
        }

        /// <summary>Splits an <see cref="IEnumerable{T}"/> into several smaller ones, each containing at most a certain number of elements.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to split.</param>
        /// <param name="size">The number of elements each group should have. The last group may contain fewer elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the elements from <see cref="source"/> split into groups of at most <see cref="source"/> elements.</returns>
        public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int size) {
            // Keep track of whether there are any items left
            bool itemsLeft;

            // Enumerate the source manually
            using (IEnumerator<T> enumerator = source.GetEnumerator()) {
                // Move to the first item
                itemsLeft = enumerator.MoveNext();

                // Make groups until out if items
                while (itemsLeft) {
                    yield return GetGroup(enumerator);
                }
            }

            // Creates a group of items from the given enumerator
            IEnumerable<T> GetGroup(IEnumerator<T> e) {
                // Keep track of the current size of this group
                int itemsRemaining = size;

                // Yield items until either the group has reached the given size or there are no more items
                while (itemsRemaining-- > 0 && itemsLeft) {
                    yield return e.Current;
                    itemsLeft = e.MoveNext();
                }
            }
        }

        /// <summary>Tries to get a value out of a dictionary. If it fails, uses a factory function to generate a new value, returning that instead and adding it to the dictionary.</summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">A function which returns a value to put in the dictionary if the key doesn't exist.</param>
        /// <returns>The existing item with the given key in the source dictionary, or the factory-generated value if the key doesn't already exist.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> factory) {
            if (source.TryGetValue(key, out TValue value)) {
                return value;
            }

            value = factory();
            source.Add(key, value);
            return value;
        }

        /// <summary>Creates a new string containing one string repeated any number of times.</summary>
        /// <param name="input">The string to repeat</param>
        /// <param name="count">How many times to repeat it</param>
        /// <returns><see cref="input"/> repeated <see cref="count"/> times</returns>
        /// <remarks>Based on this SO answer: https://stackoverflow.com/a/3754626. </remarks>
        public static string Repeat(this string input, int count) {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            StringBuilder builder = new StringBuilder(input.Length * count);
            for (int i = 0; i < count; i++) {
                builder.Append(input);
            }

            return builder.ToString();

        }

        /// <summary>Wraps this object instance into an <see cref="IEnumerable{T}"/> consisting of a single item.</summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="item">The instance that will be wrapped.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> consisting of a single item.</returns>
        /// <remarks>Based on this SO question: https://stackoverflow.com/q/1577822. </remarks>
        public static IEnumerable<T> Yield<T>(this T item) {
            yield return item;
        }

        /// <summary>Prepends an item to the beginning of an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="T">The type of elements in the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="item">The item to prepend to the source.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> with the item at the start of it.</returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item) {
            return item.Yield().Concat(source);
        }

        /// <summary>Appends an item to the end of an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="T">The type of elements in the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="item">The item to append to the source.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> with the item at the end of it.</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item) {
            return source.Concat(item.Yield());
        }
    }
}

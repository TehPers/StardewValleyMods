using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehCore.Enums;

namespace TehCore {
    public static partial class Extensions {

        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <see cref="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => new HashSet<TSource>(source.ToArray());

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) => source.ToDictionary(kv => kv.Key, kv => kv.Value);

        public static Season? ToSeason(string s) {
            switch (s.ToLower()) {
                case "spring":
                    return Season.Spring;
                case "summer":
                    return Season.Summer;
                case "fall":
                    return Season.Fall;
                case "winter":
                    return Season.Winter;
                default:
                    return null;
            }
        }

        public static Weather ToWeather(bool raining) => raining ? Weather.Rainy : Weather.Sunny;

        public static WaterType? ToWaterType(int type) {
            switch (type) {
                case -1:
                    return WaterType.Both;
                case 0:
                    return WaterType.River;
                case 1:
                    return WaterType.Lake;
                default:
                    return null;
            }
        }

        public static int ToInt(this WaterType type) => type == WaterType.Both ? -1 : (type == WaterType.River ? 0 : 1);

        /// <summary>Safely casts an object to another type with a fallback if the cast fails</summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="o">The object to cast</param>
        /// <param name="fallback">The fallback value if the cast fails</param>
        /// <returns>If the cast succeeds, <see cref="o"/> as <see cref="T"/>, otherwise <see cref="fallback"/></returns>
        public static T As<T>(this object o, T fallback = default(T)) => o is T t ? t : fallback;

        /// <summary>Retrieves a value from a <see cref="IDictionary{TKey,TValue}"/> with the given fallback value</summary>
        /// <typeparam name="TKey">The <see cref="IDictionary{TKey,TValue}"/>'s key type</typeparam>
        /// <typeparam name="TVal">The <see cref="IDictionary{TKey,TValue}"/>'s value type</typeparam>
        /// <param name="dict">The dictionary to try to retrieve the value from</param>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="fallback">The fallback value if the key doesn't exist in the dictionary</param>
        /// <returns>If the key exists in <see cref="dict"/>, the value associated with <see cref="key"/>, otherwise <see cref="fallback"/></returns>
        public static TVal GetDefault<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key, TVal fallback = default(TVal)) => dict.ContainsKey(key) ? dict[key] : fallback;

        public static void Shuffle<T>(this IList<T> list) => list.Shuffle(new Random());
        public static void Shuffle<T>(this IList<T> list, Random rand) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T Choose<T>(this IEnumerable<KeyValuePair<T, double>> elements) => elements.Choose(new Random());
        public static T Choose<T>(this IEnumerable<KeyValuePair<T, double>> elements, Random rand) {
            WeightedAuto<T> choice = elements.Select(kv => new WeightedAuto<T>(kv.Key, kv.Value)).Choose(rand);
            return choice == null ? default(T) : choice.Element;
        }

        public static T Choose<T>(this IEnumerable<T> entries) where T : IWeighted => entries.Choose(new Random());
        public static T Choose<T>(this IEnumerable<T> entries, Random rand) where T : IWeighted {
            entries = entries.ToList();
            double totalWeight = entries.Sum(entry => entry.GetWeight());
            double n = rand.NextDouble();
            foreach (T entry in entries) {
                double chance = entry.GetWeight() / totalWeight;
                if (n < chance) return entry;
                else n -= chance;
            }
            throw new ArgumentException("Enumerable must contain entries", nameof(entries));
        }

        /// <summary>Creates a new string containing one string repeated any number of times.</summary>
        /// <param name="input">The string to repeat</param>
        /// <param name="count">How many times to repeat it</param>
        /// <returns><see cref="input"/> repeated <see cref="count"/> times</returns>
        /// <remarks>Based on this SO answer: https://stackoverflow.com/a/3754626/8430206 </remarks>
        public static string Repeat(this string input, int count) {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            StringBuilder builder = new StringBuilder(input.Length * count);
            for (int i = 0; i < count; i++) {
                builder.Append(input);
            }

            return builder.ToString();

        }

        /// <summary>Tries to parse a JSON file and returns null if it fails.</summary>
        /// <typeparam name="TModel">The type of object to parse into</typeparam>
        /// <param name="helper">The <see cref="IModHelper"/> associated with the mod</param>
        /// <param name="file">The file to read</param>
        /// <returns>A <see cref="TModel"/> if successfully parsed, else null</returns>
        public static TModel TryReadJsonFile<TModel>(this IModHelper helper, string file) where TModel : class {
            try {
                return helper.ReadJsonFile<TModel>(file);
            } catch (Exception) {
                return null;
            }
        }
        
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color) {
            batch.DrawString(font, text, position + Vector2.One * Game1.pixelZoom / 2f, Color.Black);
            batch.DrawString(font, text, position, color);
        }

        public delegate void DrawStringOrShadow(Vector2 position, bool shadow);
    }
}

using StardewValley;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace TehPers.Stardew.Framework {

    internal static class Helpers {
        public static T copyFields<T>(T original, T target) {
            Type typ = original.GetType();
            while (typ != null) {
                FieldInfo[] fields = typ.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                    field.SetValue(target, field.GetValue(original));
                typ = typ.BaseType;
            }

            return target;
        }

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Game1.random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static string LocalizePath(string baseDir, string otherDir) {
            Uri baseUri = new Uri(baseDir);
            Uri otherUri = new Uri(otherDir);
            return WebUtility.UrlDecode(baseUri.MakeRelativeUri(otherUri).ToString());
        }

        public static Season? toSeason(string s) {
            switch (s.ToLower()) {
                case "spring":
                    return Season.SPRING;
                case "summer":
                    return Season.SUMMER;
                case "fall":
                    return Season.FALL;
                case "winter":
                    return Season.WINTER;
                default:
                    return null;
            }
        }

        public static Weather toWeather(bool raining) {
            return raining ? Weather.RAINY : Weather.SUNNY;
        }

        public static WaterType? convertWaterType(int type) {
            switch (type) {
                case -1:
                    return WaterType.BOTH;
                case 0:
                    return WaterType.RIVER;
                case 1:
                    return WaterType.LAKE;
                default:
                    return null;
            }
        }

        public static int convertWaterType(WaterType type) {
            return type == WaterType.BOTH ? -1 : (type == WaterType.RIVER ? 0 : 1);
        }

        public static T As<T>(this object o, T fallback = default(T)) {
            return (o is T) ? (T) o : fallback;
        }

        public static TVal GetDefault<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal fallback = default(TVal)) {
            return dict.ContainsKey(key) ? dict[key] : fallback;
        }

        public static void CopyAllFields(this object source, object dest) {
            Type srcType = source.GetType();
            if (!srcType.IsAssignableFrom(dest.GetType()))
                throw new ArgumentException("Destination type must be assignable to source type");
            while (srcType != null) {
                FieldInfo[] fields = srcType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields) {
                    field.SetValue(dest, field.GetValue(source));
                }
                srcType = srcType.BaseType;
            }
        }

        public static object ShallowCopy(this object source) {
            object copy = Activator.CreateInstance(source.GetType());
            source.CopyAllFields(copy);
            return copy;
        }
    }
}

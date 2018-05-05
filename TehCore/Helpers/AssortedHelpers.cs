using System;
using StardewModdingAPI;

namespace TehCore.Helpers {
    public static class AssortedHelpers {
        
        /// <summary>Safely casts an object to another type with a fallback if the cast fails</summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="o">The object to cast</param>
        /// <param name="fallback">The fallback value if the cast fails</param>
        /// <returns>If the cast succeeds, <see cref="o"/> as <see cref="T"/>, otherwise <see cref="fallback"/></returns>
        public static T As<T>(this object o, T fallback = default(T)) => o is T t ? t : fallback;

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehCore.Helpers.Json;

namespace TehCore.Helpers {
    public static class AssortedHelpers {
        private static readonly MethodInfo _genericCast = typeof(AssortedHelpers).GetMethod(nameof(AssortedHelpers.GenericCast), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly Type[] _primitiveTypes = { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(short), typeof(int), typeof(long), typeof(sbyte), typeof(float), typeof(string), typeof(ushort), typeof(uint), typeof(ulong) };

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

        /// <summary>Super slow way of casting things to other types. Target type only needs to be known at runtime.</summary>
        /// <param name="obj">The object to cast.</param>
        /// <param name="target">The type to cast to.</param>
        /// <returns>The casted object, or null if it couldn't be cast.</returns>
        public static object DynamicCast(this object obj, Type target) {
            Type objType = obj.GetType();

            // Check if already the target type
            if (objType == target)
                return obj;

            // Check if it can be converted
            if (obj is IConvertible && AssortedHelpers._primitiveTypes.Contains(target))
                return Convert.ChangeType(obj, target);

            // Check if they can be directly assigned
            if (target.IsAssignableFrom(objType))
                return AssortedHelpers._genericCast.MakeGenericMethod(target).Invoke(null, new[] { obj });

            return null;

        }

        private static TDest GenericCast<TSource, TDest>(TSource obj) where TSource : TDest {
            return obj;
        }

        public static bool IsNetFieldBase(this Type type) {
            // Check if objectType extends NetFieldBase
            while (type != null && type != typeof(object)) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NetFieldBase<,>))
                    return true;

                // Check base class
                type = type.BaseType;
            }

            // Doesn't extend it
            return false;
        }

        public static bool IsNetArray(this Type type) {
            // Check if objectType extends NetFieldBase
            while (type != null && type != typeof(object)) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NetArray<,>))
                    return true;

                // Check base class
                type = type.BaseType;
            }

            // Doesn't extend it
            return false;
        }
        
        public static JsonSerializerSettings Clone(this JsonSerializerSettings source) {
            return new JsonSerializerSettings {
                CheckAdditionalContent = source.CheckAdditionalContent,
                ConstructorHandling = source.ConstructorHandling,
                Context = source.Context,
                ContractResolver = source.ContractResolver,
                Converters = new List<JsonConverter>(source.Converters),
                Culture = source.Culture,
                DateFormatHandling = source.DateFormatHandling,
                DateFormatString = source.DateFormatString,
                DateParseHandling = source.DateParseHandling,
                DateTimeZoneHandling = source.DateTimeZoneHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                Error = source.Error,
                EqualityComparer = source.EqualityComparer,
                Formatting = source.Formatting,
                FloatFormatHandling = source.FloatFormatHandling,
                FloatParseHandling = source.FloatParseHandling,
                MaxDepth = source.MaxDepth,
                MetadataPropertyHandling = source.MetadataPropertyHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                NullValueHandling = source.NullValueHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                PreserveReferencesHandling = source.PreserveReferencesHandling,
                ReferenceLoopHandling = source.ReferenceLoopHandling,
                ReferenceResolverProvider = source.ReferenceResolverProvider,
                SerializationBinder = source.SerializationBinder,
                StringEscapeHandling = source.StringEscapeHandling,
                TypeNameHandling = source.TypeNameHandling,
                TraceWriter = source.TraceWriter,
                TypeNameAssemblyFormatHandling = source.TypeNameAssemblyFormatHandling,
            };
        }
    }
}

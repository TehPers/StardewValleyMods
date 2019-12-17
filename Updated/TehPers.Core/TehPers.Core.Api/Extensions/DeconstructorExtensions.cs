using System.Collections.Generic;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>Extension methods which add deconstructors to types.</summary>
    public static class DeconstructorExtensions
    {
        /// <summary>Deconstructor for <see cref="KeyValuePair{TKey,TValue}"/>.</summary>
        /// <typeparam name="TKey">The type of key in the pair.</typeparam>
        /// <typeparam name="TValue">The type of value in the pair.</typeparam>
        /// <param name="source">The source point.</param>
        /// <param name="key">The x-coordinate of the source point.</param>
        /// <param name="value">The y-coordinate of the source point.</param>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey key, out TValue value)
        {
            key = source.Key;
            value = source.Value;
        }
    }
}

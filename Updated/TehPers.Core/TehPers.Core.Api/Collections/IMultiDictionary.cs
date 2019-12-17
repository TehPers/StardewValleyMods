using System;
using System.Collections.Generic;

namespace TehPers.Core.Api.Collections
{
    /// <summary>Represents a generic collection of key/value pairs where each key represents many values.</summary>
    /// <typeparam name="TKey">The type of keys in the <see cref="IMultiDictionary{TKey,TValue}"/>.</typeparam>
    /// <typeparam name="TValue">The type of values in the <see cref="IMultiDictionary{TKey,TValue}"/>.</typeparam>
    public interface IMultiDictionary<TKey, TValue> : IDictionary<TKey, HashSet<TValue>>
    {
        /// <summary>Adds an element with the provided key and value to the <see cref="IMultiDictionary{TKey,TValue}" />.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="NotSupportedException">The <see cref="IMultiDictionary{TKey,TValue}" /> is read-only.</exception>
        void Add(TKey key, TValue value);

        /// <summary>Determines whether the <see cref="IMultiDictionary{TKey,TValue}" /> contains a specific key-value pair.</summary>
        /// <param name="key">The key of the item to locate in the <see cref="IMultiDictionary{TKey,TValue}" />.</param>
        /// <param name="value">The value of the item to locate in the <see cref="IMultiDictionary{TKey,TValue}"/>.</param>
        /// <returns><see langword="true" /> if <paramref name="value"/> is found in the set of values for <paramref name="key" />; otherwise, <see langword="false" />.</returns>
        bool Contains(TKey key, TValue value);

        /// <summary>Adds all the key-value pairs from another <see cref="IMultiDictionary{TKey,TValue}"/> into the <see cref="IMultiDictionary{TKey,TValue}"/>.</summary>
        /// <param name="other">The other <see cref="IMultiDictionary{TKey,TValue}"/>.</param>
        void AddAll(IMultiDictionary<TKey, TValue> other);
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Api.Collections
{
    /// <inheritdoc cref="IMultiDictionary{TKey,TValue}" />
    [Serializable]
    public class MultiDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>, IMultiDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TValue> valueComparer;

        /// <summary>Initializes a new instance of the <see cref="MultiDictionary{TKey, TValue}"/> class.</summary>
        public MultiDictionary()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MultiDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="keyComparer">The comparer to use for keys.</param>
        /// <param name="valueComparer">The comparer to use for values.</param>
        public MultiDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : base(keyComparer)
        {
            this.valueComparer = valueComparer;
        }

        /// <summary>Adds an element with the provided key and value to the <see cref="IMultiDictionary{TKey,TValue}" />.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="NotSupportedException">The <see cref="IMultiDictionary{TKey,TValue}" /> is read-only.</exception>
        public void Add(TKey key, TValue value)
        {
            if (!this.TryGetValue(key, out var values))
            {
                values = new HashSet<TValue>(this.valueComparer);
                this.Add(key, values);
            }

            values.Add(value);
        }

        /// <summary>Determines whether the <see cref="IMultiDictionary{TKey,TValue}" /> contains a specific key-value pair.</summary>
        /// <param name="key">The key of the item to locate in the <see cref="IMultiDictionary{TKey,TValue}" />.</param>
        /// <param name="value">The value of the item to locate in the <see cref="IMultiDictionary{TKey,TValue}"/>.</param>
        /// <returns><see langword="true" /> if <paramref name="value"/> is found in the set of values for <paramref name="key" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(TKey key, TValue value)
        {
            return this.TryGetValue(key, out var values) && values.Contains(value);
        }

        /// <summary>Adds all the key-value pairs from another <see cref="IMultiDictionary{TKey,TValue}"/> into the <see cref="IMultiDictionary{TKey,TValue}"/>.</summary>
        /// <param name="other">The other <see cref="IMultiDictionary{TKey,TValue}"/>.</param>
        public void AddAll(IMultiDictionary<TKey, TValue> other)
        {
            _ = other ?? throw new ArgumentNullException(nameof(other));

            foreach (var (key, values) in other)
            {
                foreach (var value in values)
                {
                    this.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo" /> object containing the information required to serialize the <see cref="Dictionary{TKey, TValue}" />.</param>
        /// <param name="context">A <see cref="StreamingContext" /> structure containing the source and destination of the serialized stream associated with the <see cref="Dictionary{TKey, TValue}" />.</param>
        protected MultiDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
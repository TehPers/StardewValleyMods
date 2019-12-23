using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Api.Collections
{
    /// <summary>
    /// A set where each object is stored as a weak reference. Retrieval from the set gives strong references to the objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class WeakSet<T> : ICollection<T>
        where T : class
    {
        private readonly List<WeakReference<T>> references;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakSet{T}"/> class.
        /// </summary>
        public WeakSet()
        {
            this.references = new List<WeakReference<T>>();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = this.references.Count - 1; i >= 0; i--)
            {
                var reference = this.references[i];
                if (reference.TryGetTarget(out var item))
                {
                    yield return item;
                }
                else
                {
                    this.references.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        /// <summary>Adds an item to the <see cref="WeakSet{T}" />.</summary>
        /// <param name="item">The object to add to the <see cref="WeakSet{T}" />.</param>
        /// <returns><see langword="true"/> if the item was added, <see langword="false"/> otherwise.</returns>
        public bool Add(T item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            for (var i = this.references.Count - 1; i >= 0; i--)
            {
                var reference = this.references[i];
                if (reference.TryGetTarget(out var x) && item.Equals(x))
                {
                    return false;
                }

                this.references.RemoveAt(i);
            }

            this.references.Add(new WeakReference<T>(item));
            return true;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.references.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            for (var i = this.references.Count - 1; i >= 0; i--)
            {
                var reference = this.references[i];
                if (reference.TryGetTarget(out var x) && item.Equals(x))
                {
                    return true;
                }
                else
                {
                    this.references.RemoveAt(i);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _ = array ?? throw new ArgumentNullException(nameof(array));

            var i = 0;
            while (i < this.references.Count)
            {
                var reference = this.references[i];
                if (!reference.TryGetTarget(out var item))
                {
                    this.references.RemoveAt(i);
                    continue;
                }

                array[arrayIndex + i] = item;
                i++;
            }
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            for (var i = this.references.Count - 1; i >= 0; i--)
            {
                var reference = this.references[i];
                if (reference.TryGetTarget(out var x) && item.Equals(x))
                {
                    this.references.RemoveAt(i);
                    return true;
                }
                else
                {
                    this.references.RemoveAt(i);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public int Count => this.Aggregate(0, (acc, _) => acc + 1);

        /// <inheritdoc/>
        public bool IsReadOnly => false;
    }
}

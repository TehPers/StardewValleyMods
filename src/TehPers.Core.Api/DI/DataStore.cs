using System;

namespace TehPers.Core.Api.DI
{
    /// <inheritdoc />
    public class DataStore<TData> : IDataStore<TData>
    {
        private readonly object @lock;
        private TData value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStore{T}"/> class.
        /// </summary>
        /// <param name="value">The initial value stored in this data store.</param>
        public DataStore(TData value)
        {
            this.value = value;
            this.@lock = value;
        }

        /// <inheritdoc />
        public TReturn Access<TReturn>(Func<TData, TReturn> callback)
        {
            _ = callback ?? throw new ArgumentNullException(nameof(callback));
            lock (this.@lock)
            {
               return callback(this.value);
            }
        }

        /// <inheritdoc />
        public void Replace(Func<TData, TData> callback)
        {
            _ = callback ?? throw new ArgumentNullException(nameof(callback));
            lock (this.@lock)
            {
                this.value = callback(this.value);
            }
        }
    }
}
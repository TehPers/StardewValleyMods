using System;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// A thread-safe store that allows for stateful services.
    /// </summary>
    /// <typeparam name="TData">The type of data to store.</typeparam>
    public interface IDataStore<TData>
    {
        /// <summary>
        /// Accesses the data without allowing any other thread to access the same data at the same time.
        /// </summary>
        /// <param name="callback">Callback that processes or modifies the data in this store and returns some value from it.</param>
        /// <typeparam name="TReturn">The type of value to return.</typeparam>
        /// <returns>The value returned from the callback.</returns>
        TReturn Access<TReturn>(Func<TData, TReturn> callback);

        /// <summary>
        /// Accesses and replaces the data without allowing any other thread to access the same data at the same time.
        /// </summary>
        /// <param name="callback">Callback that processes or modifies the data in this store, then returns a new value to store.</param>
        void Replace(Func<TData, TData> callback);
    }
}
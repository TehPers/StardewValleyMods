using System;

namespace TehPers.CoreMod.Api.Conflux.Responsive {
    public interface IResponsiveValue<out T> {
        /// <summary>The current value stored in this <see cref="ResponsiveValue{T}"/>.</summary>
        T Value { get; }

        /// <summary>Invoked whenever the value of this <see cref="ResponsiveValue{T}"/> is changed. <see cref="WeakReference{T}"/> is used to store event handlers to prevent memory leaks.</summary>
        event Action<T> ValueChanged;
    }
}
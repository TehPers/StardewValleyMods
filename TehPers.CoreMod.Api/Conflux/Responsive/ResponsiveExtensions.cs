using System;

namespace TehPers.CoreMod.Api.Conflux.Responsive {
    /// <remarks>Not to be confused with Reactive Extensions, an *actually good* library.</remarks>
    public static class ResponsiveExtensions {

        /// <summary>Defines a relationship which maps the source value to a new value.</summary>
        /// <typeparam name="TSource">The type of values being mapped from.</typeparam>
        /// <typeparam name="TResult">The type of values being mapped to.</typeparam>
        /// <param name="source">The source to track.</param>
        /// <param name="selector">The method which maps source values to the values of the resulting <see cref="ResponsiveValue{T}"/>.</param>
        /// <returns>A <see cref="ResponsiveValue{T}"/> which will always have a value equal to the <see cref="selector"/> being applied on the <see cref="source"/>'s value.</returns>
        public static ResponsiveValue<TResult> Select<TSource, TResult>(this ResponsiveValue<TSource> source, Func<TSource, TResult> selector) {
            return new SelectResponsiveOperator<TSource, TResult>(source, selector);
        }

        /// <summary>Defines a relationship which updates a value only if the source value meets certain criteria.</summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="source">The source to track.</param>
        /// <param name="predicate">A method which determines whether the resulting <see cref="ResponsiveValue{T}"/> should be updated.</param>
        /// <returns>A <see cref="ResponsiveValue{T}"/> which will always have a value equal to the last value the <see cref="source"/> had that met the criteria defined in the <see cref="predicate"/>. If no value has met that critera yet, the value will be the type's default value.</returns>
        public static ResponsiveValue<T> Where<T>(this ResponsiveValue<T> source, Func<T, bool> predicate) {
            return new WhereResponsiveOperator<T>(source, predicate);
        }
    }
}
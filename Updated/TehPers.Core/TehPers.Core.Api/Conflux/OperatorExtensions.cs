using System;

namespace TehPers.Core.Api.Conflux
{
    /// <summary>
    /// Extensions which provide useful operations.
    /// </summary>
    public static class OperatorExtensions
    {
        /// <summary>
        /// Forwards a function call to prevent excessive nesting of parentheses.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <param name="f">The function which transforms the source value.</param>
        /// <typeparam name="TSource">The input type for the transformation function.</typeparam>
        /// <typeparam name="TResult">The output type of the transformation function.</typeparam>
        /// <returns>The transformed value.</returns>
        public static TResult Forward<TSource, TResult>(this TSource source, Func<TSource, TResult> f)
        {
            _ = f ?? throw new ArgumentNullException(nameof(f));
            return f(source);
        }

        /// <summary>
        /// Forwards a function call to prevent excessive nesting of parentheses.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <param name="f">The function which consumes the source value.</param>
        /// <typeparam name="TSource">The input type for the consuming function.</typeparam>
        public static void Forward<TSource>(this TSource source, Action<TSource> f)
        {
            _ = f ?? throw new ArgumentNullException(nameof(f));
            f(source);
        }
    }
}

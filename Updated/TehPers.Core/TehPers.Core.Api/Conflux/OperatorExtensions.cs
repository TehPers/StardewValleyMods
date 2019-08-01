using System;

namespace TehPers.Core.Api.Conflux {
    public static class OperatorExtensions {
        public static TResult Forward<TSource, TResult>(this TSource source, Func<TSource, TResult> f) => f(source);
        public static void Forward<TSource>(this TSource source, Action<TSource> f) => f(source);
    }
}

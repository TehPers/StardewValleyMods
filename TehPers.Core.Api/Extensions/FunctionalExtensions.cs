using System;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Api.Extensions
{
    public static class FunctionalExtensions
    {
        public static TResult? Map<TSource, TResult>(this TSource? source, Func<TSource, TResult> f)
            where TSource : struct
            where TResult : struct
        {
            return source is { } s ? f(s) : null;
        }

        public static T? Or<T>(this T? source, Func<T?> f)
            where T : struct
        {
            return source is { } s ? s : f();
        }

        public static TResult? And<TSource, TResult>(this TSource? source, Func<TSource, TResult?> f)
            where TSource : struct
            where TResult : struct
        {
            return source is { } s ? f(s) : null;
        }

        public static T? Then<T>(this bool condition, Func<T> f)
            where T : struct
        {
            return condition ? f() : null;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T? source)
            where T : struct
        {
            return source is { } s ? s.Yield() : Enumerable.Empty<T>();
        }
    }
}
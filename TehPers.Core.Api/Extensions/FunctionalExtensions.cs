using System;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Api.Extensions
{
    public static class FunctionalExtensions
    {
        public static TResult? Select<TSource, TResult>(
            this TSource? source,
            Func<TSource, TResult> f
        )
            where TSource : struct
            where TResult : struct
        {
            return source is { } s ? f(s) : null;
        }

        public static T? Where<T>(this T? source, Predicate<T> f)
            where T : struct
        {
            return source switch
            {
                { } s => f(s) ? s : null,
                _ => null
            };
        }

        public static T? Or<T>(this T? source, Func<T?> f)
            where T : struct
        {
            return source switch
            {
                { } s => s,
                _ => f()
            };
        }

        public static TResult? And<TSource, TResult>(
            this TSource? source,
            Func<TSource, TResult?> f
        )
            where TSource : struct
            where TResult : struct
        {
            return source switch
            {
                { } s => f(s),
                _ => null
            };
        }

        public static T? Then<T>(this bool condition, Func<T> f)
            where T : struct
        {
            return condition ? f() : null;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T? source)
            where T : struct
        {
            return source switch
            {
                { } s => s.Yield(),
                _ => Enumerable.Empty<T>()
            };
        }
    }
}
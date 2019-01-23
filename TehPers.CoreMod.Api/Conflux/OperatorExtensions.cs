using System;
using System.Linq;

namespace TehPers.CoreMod.Api.Conflux {
    public static class OperatorExtensions {
        public static TResult Forward<TSource, TResult>(this TSource source, Func<TSource, TResult> f) => f(source);
        public static void Forward<TSource>(this TSource source, Action<TSource> f) => f(source);
    }

    public class Either<T1, T2> {
        private readonly Type _type;
        private readonly object _item;

        public Either(T1 item) {
            this._type = typeof(T1);
            this._item = item;
        }

        public Either(T2 item) {
            this._type = typeof(T2);
            this._item = item;
        }

        public TResult Case<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2) {
            if (this._type == typeof(T1)) {
                return case1((T1) this._item);
            }

            if (this._type == typeof(T2)) {
                return case2((T2) this._item);
            }

            throw new InvalidOperationException();
        }

        public static implicit operator Either<T1, T2>(T1 item) {
            return new Either<T1, T2>(item);
        }

        public static implicit operator Either<T1, T2>(T2 item) {
            return new Either<T1, T2>(item);
        }
    }
}

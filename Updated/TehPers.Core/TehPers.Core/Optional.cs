using System;
using Ninject;
using Ninject.Syntax;

namespace TehPers.Core
{
    public class Optional<T> : IOptional<T>
    {
        private readonly T _value;

        public bool HasValue { get; }
        public T Value => this.HasValue ? this._value : throw new InvalidOperationException("This does not have a value.");

        public Optional()
        {
            this.HasValue = false;
        }

        public Optional(T value)
        {
            this._value = value;
            this.HasValue = true;
        }

        public bool TryGetValue(out T value)
        {
            if (this.HasValue)
            {
                value = this._value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
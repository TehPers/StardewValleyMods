using System;
using System.Collections.Generic;
using TehPers.Core.DependencyInjection.Api;

namespace TehPers.Core.DependencyInjection
{
    internal class InjectedOptional<T> : IOptional<T>
    {
        private readonly T _value;

        public T Value => this.HasValue ? this._value : throw new InvalidOperationException("There is no value");
        public bool HasValue { get; }

        public InjectedOptional(IReadOnlyList<T> services)
        {
            if (services.Count == 1)
            {
                this._value = services[0];
                this.HasValue = true;
            }
            else
            {
                this.HasValue = false;
            }
        }

        public bool TryGetValue(out T value)
        {
            if (this.HasValue)
            {
                value = this.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
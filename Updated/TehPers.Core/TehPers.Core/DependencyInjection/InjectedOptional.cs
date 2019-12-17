using System;
using System.Collections.Generic;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.DependencyInjection
{
    internal class InjectedOptional<T> : IOptional<T>
    {
        private readonly T service;

        public T Value => this.HasValue ? this.service : throw new InvalidOperationException("There is no value");
        public bool HasValue { get; }

        public InjectedOptional(IReadOnlyList<T> services)
        {
            if (services.Count == 1)
            {
                this.service = services[0];
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
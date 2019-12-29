using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Syntax;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.DependencyInjection
{
    internal class SimpleFactory<TService> : ISimpleFactory<TService>
    {
        private readonly IResolutionRoot serviceResolver;

        public SimpleFactory(IResolutionRoot serviceResolver)
        {
            this.serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
        }

        public TService GetSingle()
        {
            return this.serviceResolver.Get<TService>();
        }

        public IEnumerable<TService> GetAll()
        {
            return this.serviceResolver.GetAll<TService>();
        }
    }
}

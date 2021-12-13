using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Syntax;
using TehPers.Core.Api.DI;

namespace TehPers.Core.DI
{
    internal class SimpleFactory<TService> : ISimpleFactory<TService>
    {
        private readonly IResolutionRoot serviceResolver;

        public SimpleFactory(IResolutionRoot resolutionRoot)
        {
            this.serviceResolver = resolutionRoot ?? throw new ArgumentNullException(nameof(resolutionRoot));
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

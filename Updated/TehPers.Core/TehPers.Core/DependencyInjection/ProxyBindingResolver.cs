using System;
using System.Collections.Generic;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace TehPers.Core.DependencyInjection
{
    public class ProxyBindingResolver : NinjectComponent, IBindingResolver
    {
        private readonly IProxyStore proxyStore;

        public ProxyBindingResolver(IProxyStore proxyStore)
        {
            this.proxyStore = proxyStore ?? throw new ArgumentNullException(nameof(proxyStore));
        }

        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
        {
            return this.proxyStore.GetProxies(service);
        }
    }
}
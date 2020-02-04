using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Components;
using Ninject.Planning.Bindings;

namespace TehPers.Core.DependencyInjection
{
    public interface IProxyStore : INinjectComponent
    {
        void AddProxy(IBinding binding, IKernel parent);

        IEnumerable<ProxyBinding> GetProxies(Type type);

        void RemoveProxies(Type service, Func<ProxyBinding, bool> predicate);
    }
}
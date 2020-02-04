using System;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.DependencyInjection
{
    public class GlobalKernel : CoreKernel, IGlobalKernel
    {
        public GlobalKernel(params INinjectModule[] modules)
            : this(new NinjectSettings(), modules)
        {
        }

        public GlobalKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
        }

        protected override void AddComponents()
        {
            base.AddComponents();
            this.Components.Add<IProxyStore, ProxyStore>();
            this.Components.Add<IBindingResolver, ProxyBindingResolver>();
        }

        public void Proxy(IBinding binding, IKernel parent)
        {
            var proxiedBindingStore = this.Components.Get<IProxyStore>();
            proxiedBindingStore.AddProxy(binding, parent);
        }

        public void Unproxy(IBinding binding)
        {
            var proxiedBindingStore = this.Components.Get<IProxyStore>();
            proxiedBindingStore.RemoveProxies(binding.Service, proxy => proxy.ParentBinding == binding);
        }

        public void Unproxy(Type service, IKernel parent)
        {
            var proxiedBindingStore = this.Components.Get<IProxyStore>();
            proxiedBindingStore.RemoveProxies(service, proxy => proxy.ParentKernel == parent);
        }
    }
}
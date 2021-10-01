using System;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using TehPers.Core.Api.DependencyInjection.Syntax;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Provides a path to register bindings and automatically register them as even thandlers.
    /// </summary>
    public abstract class WithEventsBindingRoot : IBindingRoot
    {
        private readonly IProxyBindable proxyRoot;
        private readonly IBindingRoot serviceRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsBindingRoot"/> class.
        /// </summary>
        /// <param name="proxyRoot">The proxy binding root used for binding event handlers.</param>
        /// <param name="serviceRoot">The binding root to bind the initial services.</param>
        protected WithEventsBindingRoot(IProxyBindable proxyRoot, IBindingRoot serviceRoot)
        {
            this.proxyRoot = proxyRoot ?? throw new ArgumentNullException(nameof(proxyRoot));
            this.serviceRoot = serviceRoot ?? throw new ArgumentNullException(nameof(serviceRoot));
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T> Bind<T>()
        {
            var syntax = this.serviceRoot.Bind<T>();
            return new WithEventsBindingToSyntax<T>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            var syntax = this.serviceRoot.Bind<T1, T2>();
            return new WithEventsBindingToSyntax<T1, T2>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            var syntax = this.serviceRoot.Bind<T1, T2, T3>();
            return new WithEventsBindingToSyntax<T1, T2, T3>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            var syntax = this.serviceRoot.Bind<T1, T2, T3, T4>();
            return new WithEventsBindingToSyntax<T1, T2, T3, T4>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<object> Bind(params Type[] services)
        {
            var syntax = this.serviceRoot.Bind(services);
            return new WithEventsBindingToSyntax<object>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration, services);
        }

        /// <inheritdoc/>
        public void Unbind<T>()
        {
            this.serviceRoot.Unbind<T>();
        }

        /// <inheritdoc/>
        public void Unbind(Type service)
        {
            this.serviceRoot.Unbind(service);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1> Rebind<T1>()
        {
            var syntax = this.serviceRoot.Rebind<T1>();
            return new WithEventsBindingToSyntax<T1>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            var syntax = this.serviceRoot.Rebind<T1, T2>();
            return new WithEventsBindingToSyntax<T1, T2>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            var syntax = this.serviceRoot.Rebind<T1, T2, T3>();
            return new WithEventsBindingToSyntax<T1, T2, T3>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            var syntax = this.serviceRoot.Rebind<T1, T2, T3, T4>();
            return new WithEventsBindingToSyntax<T1, T2, T3, T4>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<object> Rebind(params Type[] services)
        {
            var syntax = this.serviceRoot.Rebind(services);
            return new WithEventsBindingToSyntax<object>(this.proxyRoot, syntax.Kernel, syntax.BindingConfiguration, services);
        }

        /// <inheritdoc/>
        public void AddBinding(IBinding binding)
        {
            this.serviceRoot.AddBinding(binding);
        }

        /// <inheritdoc/>
        public void RemoveBinding(IBinding binding)
        {
            this.serviceRoot.RemoveBinding(binding);
        }
    }
}
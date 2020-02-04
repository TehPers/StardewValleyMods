using System;
using Ninject;
using Ninject.Planning.Bindings;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// A factory capable of creating any type of object based on bindings.
    /// It is the parent of every mod's kernel, however it doesn't automatically know about any bindings in them.
    /// </summary>
    public interface IGlobalKernel : IKernel
    {
        /// <summary>
        /// Adds a proxied binding to the kernel. The binding will be resolved by making a request to the binding's original parent.
        /// </summary>
        /// <param name="binding">The binding to proxy.</param>
        /// <param name="parent">The binding's parent kernel.</param>
        void Proxy(IBinding binding, IKernel parent);

        /// <summary>
        /// Removes a proxied binding from the kernel.
        /// </summary>
        /// <param name="binding">The binding to remove the proxy for.</param>
        void Unproxy(IBinding binding);

        /// <summary>
        /// Removes all proxied bindings for a particular service from the kernel.
        /// </summary>
        /// <param name="service">The type of service to remove proxies for.</param>
        /// <param name="parent">The parent kernel for those proxies.</param>
        void Unproxy(Type service, IKernel parent);
    }
}
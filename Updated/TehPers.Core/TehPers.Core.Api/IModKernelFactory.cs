using System;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Api
{
    /// <summary>Factory for creating an <see cref="IModKernel"/> for a <see cref="IMod"/>.</summary>
    public interface IModKernelFactory : IDisposable
    {
        /// <summary>
        /// Gets the global service container. Injected mod APIs can be found here.
        /// </summary>
        /// <example>
        /// To retrieve a mod's injected API:
        /// <code>
        /// SomeModApi api = kernelFactory.GlobalKernel.Get&lt;SomeModApi&gt;();
        /// </code>
        ///
        /// Other global services can be found here as well. For more advanced operations, consider making your mod service-driven by implementing <see cref="IServiceDrivenMod"/> and calling <see cref="ModExtensions.Register(TehPers.Core.Api.IServiceDrivenMod)"/>.
        /// </example>
        IResolutionRoot GlobalServices { get; }

        /// <summary>Gets the <see cref="IModKernel"/> for your <see cref="IMod"/>. This <see cref="IModKernel"/> is specific to your <see cref="IMod"/> and can only see dependencies registered to it and the global <see cref="IKernel"/>. Use <see cref="ResolutionExtensions.Get{T}(IResolutionRoot, IParameter[])"/> to get a service from the <see cref="IModKernel"/>.</summary>
        /// <param name="owner">The owner of the <see cref="IModKernel"/>.</param>
        /// <returns>The <see cref="IModKernel"/> for your <see cref="IMod"/>.</returns>
        IModKernel GetKernel(IMod owner);
    }
}
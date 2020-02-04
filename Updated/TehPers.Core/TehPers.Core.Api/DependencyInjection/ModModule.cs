using System;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <inheritdoc cref="IModModule"/>
    public abstract class ModModule : BaseModule, IModModule
    {
        /// <inheritdoc />
        public IBindingRoot GlobalProxyRoot => this.Kernel.GlobalProxyRoot;

        /// <inheritdoc/>
        public new IModKernel Kernel { get; private set; }

        /// <inheritdoc/>
        protected override IKernel KernelInstance => this.Kernel;

        /// <inheritdoc/>
        public IModKernelFactory ParentFactory => this.Kernel.ParentFactory;

        /// <inheritdoc/>
        public IMod ParentMod => this.Kernel.ParentMod;

        /// <inheritdoc/>
        public override void Unbind(Type service)
        {
            this.Kernel.Unbind(service);
        }

        /// <inheritdoc/>
        public override void OnLoad(IKernel kernel)
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            if (!(kernel is IModKernel modKernel))
            {
                throw new InvalidOperationException($"Types that inherit {nameof(ModModule)} can only be loaded into types that implement {nameof(IModKernel)}.");
            }

            this.Kernel = modKernel;
            base.OnLoad(kernel);
        }

        /// <inheritdoc/>
        public override void OnUnload(IKernel kernel)
        {
            base.OnUnload(kernel);
            this.Kernel = null;
        }
    }
}
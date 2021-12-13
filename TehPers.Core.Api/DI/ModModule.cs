using System;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.Api.DI
{
    /// <inheritdoc cref="BaseModule"/>
    /// <inheritdoc cref="IModModule"/>
    public abstract class ModModule : BaseModule, IModModule
    {
        public IBindingRoot GlobalProxyRoot => this.Kernel.GlobalProxyRoot;
        public new IModKernel Kernel { get; private set; }
        protected override IKernel KernelInstance => this.Kernel;
        public IModKernelFactory ParentFactory => this.Kernel.ParentFactory;
        public IMod ParentMod => this.Kernel.ParentMod;

        protected ModModule()
        {
            // Kernel should not be used until after `OnLoad` is called
            // TODO: is there a better way to do this?
            this.Kernel = null!;
        }

        public override void Unbind(Type service)
        {
            this.Kernel.Unbind(service);
        }

        public override void OnLoad(IKernel kernel)
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            if (kernel is not IModKernel modKernel)
            {
                throw new InvalidOperationException(
                    $"Types that inherit {nameof(ModModule)} can only be loaded into types that implement {nameof(IModKernel)}."
                );
            }

            this.Kernel = modKernel;
            base.OnLoad(kernel);
        }

        public override void OnUnload(IKernel kernel)
        {
            base.OnUnload(kernel);

            // Kernel should not be used after this point
            // TODO: is there a better way to do this?
            this.Kernel = null!;
        }
    }
}
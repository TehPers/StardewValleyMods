using System;
using Ninject;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.DependencyInjection
{
    public class ProxiedBindingRoot : BindingRoot
    {
        private readonly IGlobalKernel globalKernel;
        private readonly IKernel childKernel;

        protected override IKernel KernelInstance => this.childKernel;

        public ProxiedBindingRoot(IGlobalKernel globalKernel, IKernel childKernel)
        {
            this.globalKernel = globalKernel ?? throw new ArgumentNullException(nameof(globalKernel));
            this.childKernel = childKernel ?? throw new ArgumentNullException(nameof(childKernel));
        }

        public override void Unbind(Type service)
        {
            _ = service ?? throw new ArgumentNullException(nameof(service));
            this.globalKernel.Unproxy(service, this.childKernel);
            this.childKernel.Unbind(service);
        }

        public override void AddBinding(IBinding binding)
        {
            _ = binding ?? throw new ArgumentNullException(nameof(binding));
            this.childKernel.AddBinding(binding);
            this.globalKernel.Proxy(binding, this.childKernel);
        }

        public override void RemoveBinding(IBinding binding)
        {
            _ = binding ?? throw new ArgumentNullException(nameof(binding));
            this.globalKernel.Unproxy(binding);
            this.childKernel.RemoveBinding(binding);
        }
    }
}
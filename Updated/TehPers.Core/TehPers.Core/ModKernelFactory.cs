using System.Collections.Generic;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.DependencyInjection;
using TehPers.Core.Modules;

namespace TehPers.Core
{
    internal sealed class ModKernelFactory : IModKernelFactory
    {
        private readonly IKernel globalKernel;
        private readonly Dictionary<string, IModKernel> modKernels;

        public IResolutionRoot GlobalServices => this.globalKernel;

        public ModKernelFactory()
        {
            this.globalKernel = new StandardKernel();
            this.modKernels = new Dictionary<string, IModKernel>();

            this.RegisterGlobalServices();
        }

        private void RegisterGlobalServices()
        {
            this.globalKernel.Bind(typeof(IOptional<>))
                .To(typeof(InjectedOptional<>))
                .InTransientScope();
        }

        public IModKernel GetKernel(IMod owner)
        {
            if (!this.modKernels.TryGetValue(owner.ModManifest.UniqueID, out var modKernel))
            {
                modKernel = new ModKernel(owner, this.globalKernel, new NinjectSettings
                {
                    LoadExtensions = false
                });

                modKernel.Load(
                    new ModModule(owner, modKernel),
                    new FuncModule()
                    );
                this.modKernels.Add(owner.ModManifest.UniqueID, modKernel);
            }

            return modKernel;
        }

        public void Dispose()
        {
            foreach (var modKernel in this.modKernels.Values)
            {
                modKernel?.Dispose();
            }

            this.modKernels.Clear();
            this.globalKernel?.Dispose();
        }
    }
}

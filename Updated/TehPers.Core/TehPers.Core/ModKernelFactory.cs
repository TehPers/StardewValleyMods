using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.Core.DependencyInjection;
using TehPers.Core.Modules;

namespace TehPers.Core
{
    public sealed class ModKernelFactory : IModKernelFactory
    {
        private readonly GlobalKernel globalKernel;
        private readonly Dictionary<IManifest, IModKernel> modKernels;
        private readonly HashSet<Func<IManifest, INinjectModule>> modModuleFactories;

        public IResolutionRoot GlobalServices => this.globalKernel;

        public ModKernelFactory()
        {
            this.globalKernel = new GlobalKernel();
            this.modKernels = new Dictionary<IManifest, IModKernel>();
            this.modModuleFactories = new HashSet<Func<IManifest, INinjectModule>>();

            this.RegisterGlobalServices();
        }

        private void RegisterGlobalServices()
        {
            this.globalKernel.Bind(typeof(IOptional<>))
                .To(typeof(InjectedOptional<>))
                .InTransientScope();
            this.globalKernel.Bind(typeof(ISimpleFactory<>))
                .To(typeof(SimpleFactory<>))
                .InSingletonScope();
            this.globalKernel.Bind<IModKernelFactory>()
                .ToConstant(this)
                .InSingletonScope();
        }

        public void LoadIntoModKernels(Func<IManifest, INinjectModule> moduleFactory)
        {
            _ = moduleFactory ?? throw new ArgumentNullException(nameof(moduleFactory));

            this.modModuleFactories.Add(moduleFactory);
            foreach (var (manifest, kernel) in this.modKernels)
            {
                kernel.Load(moduleFactory(manifest));
            }
        }

        public IModKernel GetKernel(IMod owner)
        {
            if (!this.modKernels.TryGetValue(owner.ModManifest, out var modKernel))
            {
                modKernel = new ModKernel(owner, this.globalKernel, this, new NinjectSettings
                {
                    LoadExtensions = false,
                });

                modKernel.Load(
                    new ModServicesModule(owner, modKernel),
                    new FuncModule()
                );

                foreach (var factory in this.modModuleFactories)
                {
                    modKernel.Load(factory(owner.ModManifest));
                }

                this.modKernels.Add(owner.ModManifest, modKernel);
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
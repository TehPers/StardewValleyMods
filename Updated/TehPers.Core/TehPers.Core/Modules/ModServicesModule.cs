using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.Modules
{
    internal class ModServicesModule : NinjectModule
    {
        private readonly IMod mod;
        private readonly IModKernel modKernel;

        public ModServicesModule(IMod mod, IModKernel modKernel)
        {
            this.mod = mod;
            this.modKernel = modKernel;
        }

        public override void Load()
        {
            // SMAPI types
            this.Bind(this.mod.GetType(), typeof(IMod))
                .ToConstant(this.mod)
                .InSingletonScope();
            this.Bind<IMonitor>()
                .ToConstant(this.mod.Monitor)
                .InSingletonScope();
            this.Bind<IModHelper>()
                .ToConstant(this.mod.Helper)
                .InSingletonScope();
            this.Bind<IManifest>()
                .ToConstant(this.mod.ModManifest)
                .InSingletonScope();

            // The mod's kernel
            this.Bind<IModKernel>()
                .ToConstant(this.modKernel)
                .InSingletonScope();
        }
    }
}

using Ninject;
using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.DependencyInjection.Api;

namespace TehPers.Core
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log("Hello, world!");

            this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                if (this.Helper.ModRegistry.GetApi<IDependencyInjectionApi>("TehPers.Core.DependencyInjection") is IDependencyInjectionApi diApi)
                {
                    this.LoadModBindings(diApi.GetModKernel(this));
                }
            };

            this.Monitor.Log("Registering dependencies", LogLevel.Info);
        }

        private void LoadGlobalModules(IKernel globalKernel)
        {

        }

        private void LoadModBindings(IKernel modKernels)
        {
            modKernels.Load(new INinjectModule[] {
                new CoreModule(this),
                new Modules.CoreModule(this),
            });
        }
    }

    internal class CoreModule : NinjectModule
    {
        private readonly IMod _mod;

        public CoreModule(IMod mod)
        {
            this._mod = mod;
        }

        public override void Load()
        {
            foreach (IModInfo modInfo in this._mod.Helper.ModRegistry.GetAll())
            {
                this.Bind<IModInfo>().ToConstant(modInfo).InSingletonScope();
                this.Bind<IManifest>().ToConstant(modInfo.Manifest).InSingletonScope();
            }

            this.Bind(typeof(IOptional<>)).To(typeof(Optional<>)).InTransientScope();
        }
    }
}

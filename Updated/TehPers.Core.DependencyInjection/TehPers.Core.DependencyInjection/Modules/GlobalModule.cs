using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.DependencyInjection.Api;

namespace TehPers.Core.DependencyInjection.Modules
{
    public class GlobalModule : NinjectModule
    {
        private readonly IMod _mod;
        private readonly IDependencyInjectionApi _diApi;

        public GlobalModule(IMod mod, IDependencyInjectionApi diApi)
        {
            this._mod = mod;
            this._diApi = diApi;
        }

        public override void Load()
        {
            this.Bind<IDependencyInjectionApi>().ToConstant(this._diApi).InSingletonScope();
            this.Bind(typeof(IOptional<>)).To(typeof(InjectedOptional<>)).InTransientScope();

            // Bind all mod APIs to their own types
            foreach (IModInfo modInfo in this._mod.Helper.ModRegistry.GetAll())
            {
                if (!(this._mod.Helper.ModRegistry.GetApi(modInfo.Manifest.UniqueID) is object modApi))
                {
                    continue;
                }

                this._mod.Monitor.Log($"Binding API for '{modInfo.Manifest.UniqueID}' to itself.", LogLevel.Trace);
                this.Bind(modApi.GetType()).ToConstant(modApi).InSingletonScope();
            }
        }
    }
}
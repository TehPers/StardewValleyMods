using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework
{
    public class ModEntry : Mod, IServiceDrivenMod
    {
        public override void Entry(IModHelper helper)
        {
            this.Register();
        }

        public void GameLoaded(IModKernel modKernel)
        {
            var modInit = modKernel.Get<ModInit>();
            modInit.Init();
        }

        public void RegisterServices(IModKernel modKernel)
        {
            modKernel.Bind<ModInit>().ToSelf().InSingletonScope();
            modKernel.Bind<FishingOverrideService>().ToSelf().InSingletonScope();
            modKernel.AddEventHandler<FishingOverrideService>();
            modKernel.Bind<FishingApi>().ToSelf().InSingletonScope();
            modKernel.ExposeService<IFishingApi, FishingApi>();
        }
    }
}

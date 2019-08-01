using System;
using StardewModdingAPI;
using TehPers.Core.DependencyInjection.Api;
using TehPers.Core.DependencyInjection.Api.Extensions;

namespace TehPers.FishingFramework
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                if (!(this.Helper.ModRegistry.GetApi<IDependencyInjectionApi>("TehPers.Core.DependencyInjection")?.GetModKernel(this) is IModKernel modKernel))
                {
                    throw new Exception("Dependency injection module is not installed");
                }

                this.RegisterServices(modKernel);
            };
        }

        private void RegisterServices(IModKernel kernel)
        {
            kernel.BindModTypes();
            kernel.Bind<FishingOverrideService>().ToSelf().InSingletonScope();
            // kernel.Global.Bind<IFishingApi>().To<FishingApi>().InSingletonScope();

            this.Helper.ReadConfig<>()

            kernel.RegisterEvents<FishingOverrideService>();
        }
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            kernel.BindCustomModApi<object>("Pathoschild.ContentPatcher").InSingletonScope();
            // kernel.Global.Bind<IFishingApi>().To<FishingApi>().InSingletonScope();

            kernel.RegisterEvents<FishingOverrideService>();
        }
    }
}

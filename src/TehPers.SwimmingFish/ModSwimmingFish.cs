using HarmonyLib;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Setup;
using TehPers.SwimmingFish.Services;

namespace TehPers.SwimmingFish
{
    /// <summary>
    /// The mod entry class.
    /// </summary>
    public class ModSwimmingFish : Mod
    {
        private IModKernel? kernel;

        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            if (ModServices.Factory is not { } kernelFactory)
            {
                this.Monitor.Log(
                    "Core mod seems to not be loaded. Aborting setup - this mod is effectively disabled.",
                    LogLevel.Error
                );

                return;
            }

            // Register services
            this.kernel = kernelFactory.GetKernel(this);

            // Initialization
            this.kernel.Bind<ISetup>().To<FishRenderer>().InSingletonScope();
            this.kernel.Bind<ISetup, FishTracker>().To<FishTracker>().InSingletonScope();
            this.kernel.Bind<ISetup, WaterDrawnTracker>().ToMethod(WaterDrawnTracker.Create).InSingletonScope();

            // Resources
            this.kernel.Bind<Harmony>()
                .ToMethod(context => {
                    var manifest = context.Kernel.Get<IManifest>();
                    return new(manifest.UniqueID);
                });
        }
    }
}

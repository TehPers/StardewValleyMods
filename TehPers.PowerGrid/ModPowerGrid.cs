using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;

namespace TehPers.PowerGrid
{
    /// <summary>
    /// Entry class for Power Grid.
    /// </summary>
    /// <inheritdoc cref="Mod"/>
    public class ModPowerGrid : Mod
    {
        private IModKernel? kernel;

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

            this.kernel = kernelFactory.GetKernel(this);
            this.kernel.Load<PowerGridModule>();

            var startup = this.kernel.Get<Startup>();
            startup.Initialize();
        }
    }
}
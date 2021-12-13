using TehPers.Core.Api.DI;
using TehPers.Core.Api.Setup;
using TehPers.PowerGrid.World;
using TehPers.PowerGrid.Services;
using TehPers.PowerGrid.Services.Setup;

namespace TehPers.PowerGrid
{
    internal class PowerGridModule : ModModule
    {
        public override void Load()
        {
            // Setup
            this.Bind<ISetup>().To<NetworkWatcher>().InSingletonScope();

            // Services
            this.Bind<NetworkFinder>().ToSelf().InSingletonScope();
        }
    }
}
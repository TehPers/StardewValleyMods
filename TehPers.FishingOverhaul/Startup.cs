using System;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Setup;

namespace TehPers.FishingOverhaul
{
    internal sealed class Startup
    {
        private readonly IMonitor monitor;
        private readonly ISetup[] setupServices;

        public Startup(IMonitor monitor, ISetup[] setupServices)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.setupServices = setupServices ?? throw new ArgumentNullException(nameof(setupServices));
        }

        public void Initialize()
        {
            this.monitor.Log("Setting up fishing services.", LogLevel.Info);
            foreach (var service in this.setupServices)
            {
                service.Setup();
            }
        }
    }
}
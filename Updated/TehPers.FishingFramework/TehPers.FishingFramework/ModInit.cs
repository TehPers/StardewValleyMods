using StardewModdingAPI;

namespace TehPers.FishingFramework
{
    internal class ModInit
    {
        private readonly IMonitor monitor;

        public ModInit(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        public void Init()
        {
            this.monitor.Log("Mod initialized", LogLevel.Info);
        }
    }
}
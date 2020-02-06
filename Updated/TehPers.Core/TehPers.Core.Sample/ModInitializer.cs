using System;
using StardewModdingAPI;
using TehPers.Core.Api.Configuration;

namespace TehPers.Core.Sample
{
    public class ModInitializer
    {
        private readonly IMonitor monitor;
        private readonly IConfiguration<ModConfig> config;

        public ModInitializer(IMonitor monitor, IConfiguration<ModConfig> config)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Initialize()
        {
            this.monitor.Log(this.config.Value.Message, LogLevel.Alert);
        }
    }
}
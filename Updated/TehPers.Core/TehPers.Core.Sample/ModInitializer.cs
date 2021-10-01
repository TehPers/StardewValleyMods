using System;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.Configuration;
using TehPers.Core.Api.Items;

namespace TehPers.Core.Sample
{
    public class ModInitializer
    {
        private readonly IMonitor monitor;
        private readonly IConfiguration<ModConfig> config;
        private readonly IIndexRegistry indexRegistry;

        public ModInitializer(IMonitor monitor, IConfiguration<ModConfig> config, [Named("objects")] IIndexRegistry indexRegistry)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.indexRegistry = indexRegistry ?? throw new ArgumentNullException(nameof(indexRegistry));
        }

        public void Initialize()
        {
            this.monitor.Log(this.config.Value.Message, LogLevel.Alert);
            this.indexRegistry.Reserve(new NamespacedId("TehPers.Core.SampleMod", "test"));
        }
    }
}
using System;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Json;
using TehPers.Core.Json;

namespace TehPers.Core.Modules
{
    public class ModJsonModule : ModModule
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        public ModJsonModule(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public override void Load()
        {
            this.GlobalProxyRoot.Bind<IJsonProvider>()
                .To<CommentedJsonProvider>()
                .InSingletonScope();
        }
    }
}
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Items;
using TehPers.Core.Items;

namespace TehPers.Core.Modules
{
    internal class ItemProvidersModule : ModModule
    {
        public override void Load()
        {
            this.GlobalProxyRoot.Bind<IGlobalItemProvider>()
                .To<GlobalItemProvider>()
                .InSingletonScope();

            this.GlobalProxyRoot.Bind<IItemProvider>()
                .To<SObjectItemProvider>()
                .InSingletonScope();
        }
    }
}
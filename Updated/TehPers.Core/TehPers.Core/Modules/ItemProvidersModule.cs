using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Items;

namespace TehPers.Core.Modules
{
    internal class ItemProvidersModule : ModModule
    {
        public override void Load()
        {
            this.GlobalRoot.Bind<IGlobalItemProvider>()
                .To<GlobalItemProvider>()
                .InSingletonScope();
            this.Bind<SObjectItemProvider>()
                .ToSelf()
                .InSingletonScope();
            // this.AddEventHandler<SObjectItemProvider>();
            this.ExposeService<IItemProvider, SObjectItemProvider>();
        }
    }
}

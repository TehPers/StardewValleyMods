using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Items;

namespace TehPers.Core.Modules
{
    public class IndexRegistriesModule : ModModule
    {
        public override void Load()
        {
            // Objects
            this.Bind<SObjectIndexRegistry>().ToSelf().InSingletonScope();
            this.GlobalProxyRoot.BindProxy<IIndexRegistry, SObjectIndexRegistry>().Named("objects");
            this.AddEventHandler<SObjectIndexRegistry>();
        }
    }
}
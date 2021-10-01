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
            this.WithEvents()
                .GlobalProxyRoot
                .Bind<IIndexRegistry>()
                .To<IndexRegistry>()
                .InSingletonScope()
                .Named("objects")
                .WithConstructorArgument("registryKey", "objects")
                .WithConstructorArgument("randomOffset", 1000000);
        }
    }
}
using Ninject;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.Core.Content;
using TehPers.Core.Integrations.DynamicGameAssets;
using TehPers.Core.Integrations.JsonAssets;
using TehPers.Core.Items;
using TehPers.Core.Setup;

namespace TehPers.Core.Modules
{
    internal class CoreServicesModule : ModModule
    {
        public override void Load()
        {
            // Startup services
            this.Bind<ISetup>().To<NamespaceSetup>().InSingletonScope();
            this.Bind<ISetup>()
                .ToMethod(context => context.Kernel.Get<AssetTracker>())
                .InSingletonScope();

            // Services
            this.GlobalProxyRoot.Bind<INamespaceRegistry>()
                .To<NamespaceRegistry>()
                .InSingletonScope();
            this.GlobalProxyRoot.Bind<IAssetTracker>().To<AssetTracker>().InSingletonScope();

            // Namespaces
            this.GlobalProxyRoot.Bind<INamespaceProvider>()
                .To<StardewValleyNamespace>()
                .InSingletonScope();
            this.GlobalProxyRoot.Bind<INamespaceProvider>()
                .To<DynamicGameAssetsNamespace>()
                .InSingletonScope();
            this.GlobalProxyRoot.Bind<INamespaceProvider>()
                .To<JsonAssetsNamespace>()
                .InSingletonScope();

            // Mod APIs
            this.BindForeignModApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets")
                .InSingletonScope();
            this.BindForeignModApi<IJsonAssetsApi>("spacechase0.JsonAssets").InSingletonScope();
        }
    }
}
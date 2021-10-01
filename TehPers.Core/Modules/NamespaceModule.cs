using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using TehPers.Core.Items;

namespace TehPers.Core.Modules
{
    internal class NamespaceModule : ModModule
    {
        public override void Load()
        {
            this.GlobalProxyRoot.Bind<INamespaceProvider>().To<StardewValleyNamespace>().InSingletonScope();
            this.GlobalProxyRoot.Bind<INamespaceRegistry>().To<NamespaceRegistry>().InSingletonScope();
        }
    }
}
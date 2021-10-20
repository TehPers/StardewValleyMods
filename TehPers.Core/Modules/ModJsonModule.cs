using TehPers.Core.Api.DI;
using TehPers.Core.Api.Json;
using TehPers.Core.Json;

namespace TehPers.Core.Modules
{
    public class ModJsonModule : ModModule
    {
        public override void Load()
        {
            this.GlobalProxyRoot.Bind<IJsonProvider>()
                .To<CommentedJsonProvider>()
                .InSingletonScope();
        }
    }
}
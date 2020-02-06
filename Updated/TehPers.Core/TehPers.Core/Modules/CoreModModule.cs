using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.DependencyInjection.Lifecycle;

namespace TehPers.Core.Modules
{
    internal class CoreModModule : ModModule
    {
        public override void Load()
        {
            // Mod services
            this.Bind<LifecycleService>().ToSelf().InSingletonScope();
        }
    }
}
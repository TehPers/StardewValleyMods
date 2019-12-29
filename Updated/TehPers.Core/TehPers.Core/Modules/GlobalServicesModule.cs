using Ninject.Modules;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Items;
using TehPers.Core.Items;

namespace TehPers.Core.Modules
{
    public class GlobalServicesModule : NinjectModule
    {
        private readonly IModKernel modKernel;

        public GlobalServicesModule(IModKernel modKernel)
        {
            this.modKernel = modKernel;
        }

        public override void Load()
        {
            this.Bind<IGlobalItemProvider>().To<GlobalItemProvider>().InSingletonScope();
        }
    }
}

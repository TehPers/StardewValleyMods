using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Json;
using TehPers.Core.DependencyInjection;
using TehPers.Core.Json;

namespace TehPers.Core.Modules
{
    internal class ModModule : NinjectModule
    {
        private readonly IMod mod;
        private readonly IModKernel modKernel;

        public ModModule(IMod mod, IModKernel modKernel)
        {
            this.mod = mod;
            this.modKernel = modKernel;
        }

        public override void Load()
        {
            this.Bind<IMod>()
                .ToConstant(this.mod)
                .InTransientScope();
            this.Bind<IMonitor>()
                .ToConstant(this.mod.Monitor)
                .InTransientScope();
            this.Bind<IModHelper>()
                .ToConstant(this.mod.Helper)
                .InTransientScope();
            this.Bind<IManifest>()
                .ToConstant(this.mod.ModManifest)
                .InTransientScope();
            this.Bind<IModKernel>()
                .ToConstant(this.modKernel)
                .InTransientScope();
            this.Bind<ICommentedJsonApi>()
                .To<CommentedJsonApi>()
                .InSingletonScope();
            this.Bind(typeof(IOptional<>))
                .To(typeof(InjectedOptional<>))
                .InTransientScope();
        }
    }
}

using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Json;
using TehPers.Core.Content;
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
            // SMAPI types
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

            // The mod's kernel
            this.Bind<IModKernel>()
                .ToConstant(this.modKernel)
                .InTransientScope();

            // Json
            this.Bind<ICommentedJsonApi>()
                .To<CommentedJsonApi>()
                .InSingletonScope();
            
            // Content
            this.Bind<IAssetProvider>()
                .To<ModAssetProvider>()
                .InSingletonScope()
                .WithMetadata(nameof(ContentSource), ContentSource.ModFolder);
            this.Bind<IAssetProvider>()
                .To<GameAssetProvider>()
                .InSingletonScope()
                .WithMetadata(nameof(ContentSource), ContentSource.GameContent);

            // DI-related types
            this.Bind(typeof(IOptional<>))
                .To(typeof(InjectedOptional<>))
                .InTransientScope();
            this.Bind(typeof(ISimpleFactory<>))
                .To(typeof(SimpleFactory<>))
                .InSingletonScope();
        }
    }
}

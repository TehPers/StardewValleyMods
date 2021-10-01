using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Content;
using TehPers.Core.DependencyInjection;

namespace TehPers.Core.Modules
{
    internal class CoreApiModModule : NinjectModule
    {
        public override void Load()
        {
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
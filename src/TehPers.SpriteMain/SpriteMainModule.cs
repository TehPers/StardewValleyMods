using HarmonyLib;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Setup;
using TehPers.SpriteMain.Integrations.GenericModConfigMenu;
using TehPers.SpriteMain.Patches;

namespace TehPers.SpriteMain
{
    internal class SpriteMainModule : ModModule
    {
        public override void Load()
        {
            // Initialization
            this.Bind<SpriteBatchPatcher, ISetup>()
                .ToMethod(SpriteBatchPatcher.Create)
                .InSingletonScope();

            // Resources/services
            this.Bind<Harmony>()
                .ToMethod(ctx => new(ctx.Kernel.Get<IManifest>().UniqueID))
                .InSingletonScope();

            // Config
            this.Bind<ISetup, ModConfigManager>()
                .To<ModConfigManager>()
                .InSingletonScope()
                .WithConstructorArgument("path", "config/config.json");

            // Foreign APIs
            this.BindForeignModApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")
                .InSingletonScope();
        }
    }
}
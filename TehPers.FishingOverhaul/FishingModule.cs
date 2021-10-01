using Ninject;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Gui;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;
using TehPers.FishingOverhaul.Loading;
using TehPers.FishingOverhaul.Services;
using TehPers.FishingOverhaul.Setup;

namespace TehPers.FishingOverhaul
{
    internal class FishingModule : ModModule
    {
        public override void Load()
        {
            // Initialization
            this.Bind<Startup>().ToSelf().InSingletonScope();
            this.Bind<ISetup>().To<FishingHudRenderer>().InSingletonScope();
            this.Bind<ISetup, FishingRodOverrider>().To<FishingRodOverrider>().InSingletonScope();
            this.Bind<ISetup>().To<FishLoader>().InSingletonScope();
            this.Bind<ISetup>().To<FishingMessageHandler>().InSingletonScope();
            this.Bind<ISetup>().To<GenericModConfigMenuSetup>().InSingletonScope();
            this.Bind<ISetup>().To<FishingController>().InSingletonScope();

            // Resources/services
            this.Bind<IFishingHelper>().To<FishingHelper>().InSingletonScope();
            this.Bind<ICustomBobberBarFactory>().To<CustomBobberBarFactory>().InSingletonScope();
            this.Bind<FishingData>().ToConstant(new FishingData()).InSingletonScope();
            this.Bind<TrashData>().ToMethod(_ => TrashData.GetDefaultTrashData()).InSingletonScope();
            this.Bind<FishingTracker>().ToSelf().InSingletonScope();

            // Configs
            this.BindConfiguration<GeneralConfig>("config/general.json");
            this.BindConfiguration<HudConfig>("config/hud.json");
            this.BindConfiguration<FishConfig>("config/fish.json");
            this.BindConfiguration<TreasureConfig>("config/treasure.json");

            // Foreign APIs
            this.BindForeignModApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")
                .InSingletonScope();
        }

        private void BindConfiguration<T>(string path)
            where T : class, IModConfig, new()
        {
            this.Bind<ConfigManager<T>>().ToSelf().InSingletonScope().WithConstructorArgument("path", path);
            this.Bind<IModConfig, T>().ToMethod(context => context.Kernel.Get<ConfigManager<T>>().Load())
                .InSingletonScope();
        }
    }
}
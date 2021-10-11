using System;
using System.Collections.Generic;
using ContentPatcher;
using HarmonyLib;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.ContentPacks;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;
using TehPers.FishingOverhaul.Services;
using TehPers.FishingOverhaul.Services.Setup;

namespace TehPers.FishingOverhaul
{
    internal class FishingModule : ModModule
    {
        public override void Load()
        {
            // Initialization
            this.Bind<Startup>().ToSelf().InSingletonScope();
            this.Bind<ISetup>().To<FishingHudRenderer>().InSingletonScope();
            this.Bind<ISetup>().To<GenericModConfigMenuSetup>().InSingletonScope();
            this.Bind<ISetup>().ToMethod(FishingRodPatcher.Create).InSingletonScope();
            this.Bind<ISetup>().To<DefaultContentReloader>().InSingletonScope();
            this.Bind<ISetup>().To<ContentPatcherSetup>().InSingletonScope();
            this.Bind<ISetup>().To<DefaultCustomEvents>().InSingletonScope();
            // TODO: this.Bind<ISetup>().To<FishingMessageHandler>().InSingletonScope();

            // Resources/services
            this.Bind<IFishingApi, ISimplifiedFishingApi, FishingApi>()
                .ToMethod(
                    context => new FishingApi(
                        context.Kernel.Get<IMonitor>(),
                        context.Kernel.Get<IManifest>(),
                        context.Kernel.Get<INamespaceRegistry>(),
                        context.Kernel.Get<FishConfig>(),
                        context.Kernel.Get<TreasureConfig>(),
                        context.Kernel.Get<Func<IEnumerable<IFishingContentSource>>>(),
                        context.Kernel.Get<EntryManagerFactory<FishEntry, FishAvailabilityInfo>>(),
                        context.Kernel.Get<EntryManagerFactory<TrashEntry, AvailabilityInfo>>(),
                        context.Kernel.Get<EntryManagerFactory<TreasureEntry, AvailabilityInfo>>()
                    )
                )
                .InSingletonScope();
            this.Bind<ICustomBobberBarFactory>().To<CustomBobberBarFactory>().InSingletonScope();
            this.Bind<FishingTracker>().ToSelf().InSingletonScope();
            this.Bind<Harmony>()
                .ToMethod(
                    context =>
                    {
                        var manifest = context.Kernel.Get<IManifest>();
                        return new(manifest.UniqueID);
                    }
                )
                .InSingletonScope();
            this.Bind(typeof(ChanceCalculatorFactory<>)).ToSelf().InSingletonScope();
            this.Bind(typeof(EntryManagerFactory<,>)).ToSelf().InSingletonScope();

            // Configs
            this.BindConfiguration<HudConfig>("config/hud.json");
            this.BindConfiguration<FishConfig>("config/fish.json");
            this.BindConfiguration<TreasureConfig>("config/treasure.json");

            // Content
            this.GlobalProxyRoot.Bind<IFishingContentSource>()
                .To<ContentPackSource>()
                .InSingletonScope();
            this.GlobalProxyRoot.Bind<IFishingContentSource>()
                .To<DefaultFishingSource>()
                .InSingletonScope();

            // Foreign APIs
            this.BindForeignModApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")
                .InSingletonScope();
            this.BindForeignModApi<IContentPatcherAPI>("Pathoschild.ContentPatcher")
                .InSingletonScope();
        }

        private void BindConfiguration<T>(string path)
            where T : class, IModConfig, new()
        {
            this.Bind<ConfigManager<T>>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("path", path);
            this.Bind<IModConfig, T>()
                .ToMethod(context => context.Kernel.Get<ConfigManager<T>>().Load())
                .InSingletonScope();
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Ninject;
using StardewModdingAPI;
using StardewValley.Tools;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Providers;
using TehPers.FishingFramework.Config;
using TehPers.FishingFramework.Providers;

namespace TehPers.FishingFramework
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Type is instantiated by SMAPI")]
    public class ModEntry : Mod, IServiceDrivenMod
    {
        public override void Entry(IModHelper helper)
        {
            this.Register();
        }

        public void GameLoaded(IModKernel modKernel)
        {
            var modInit = modKernel.Get<ModInit>();
            modInit.Init();
        }

        public void RegisterServices(IModKernel modKernel)
        {
            modKernel.Bind<ModInit>()
                .ToSelf()
                .InSingletonScope();
            modKernel.Bind<FishingOverrideService>()
                .ToSelf()
                .InSingletonScope();
            modKernel.Bind<IDataStore<ConditionalWeakTable<FishingRod, FishingRodData>>>()
                .ToConstant(new DataStore<ConditionalWeakTable<FishingRod, FishingRodData>>(new ConditionalWeakTable<FishingRod, FishingRodData>()))
                .WhenInjectedInto<FishingOverrideService>()
                .InSingletonScope();
            modKernel.AddEventHandler<FishingOverrideService>();
            modKernel.Bind<IDefaultFishProvider>()
                .To<DefaultFishProvider>()
                .InSingletonScope();
            modKernel.Bind<IDefaultTreasureProvider>()
                .To<DefaultTreasureProvider>()
                .InSingletonScope();
            modKernel.Bind<IDefaultTrashProvider>()
                .To<DefaultTrashProvider>()
                .InSingletonScope();

            // Exposed types
            modKernel.GlobalProxyRoot.Bind<IFishingApi, FishingApi>()
                .To<FishingApi>()
                .InSingletonScope();
            modKernel.GlobalProxyRoot.Bind<IFishProvider, BaseFishProvider>()
                .To<BaseFishProvider>()
                .InSingletonScope();

            // Configs
            modKernel.BindConfiguration<FishConfiguration>("Configs/General/fish.json");
            modKernel.BindConfiguration<TreasureConfiguration>("Configs/General/treasure.json");
            modKernel.BindConfiguration<DifficultyConfiguration>("Configs/General/difficulty.json");
            modKernel.BindConfiguration<FishEntriesConfiguration>("Configs/Data/fishEntries.json");
            modKernel.BindConfiguration<TreasureEntriesConfiguration>("Configs/Data/treasureEntries.json");
            modKernel.BindConfiguration<FishTraitsConfiguration>("Configs/Data/fishTraits.json");
        }
    }
}
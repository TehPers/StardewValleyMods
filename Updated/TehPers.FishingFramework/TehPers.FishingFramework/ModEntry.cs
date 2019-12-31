using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Ninject;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.Core.Api.Extensions;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Config;
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
            modKernel.Bind<IDefaultFishProvider, IDefaultTreasureProvider, IDefaultTrashProvider>()
                .To<DefaultFishingProvider>()
                .InSingletonScope();

            // Exposed types
            modKernel.Bind<FishingApi>()
                .ToSelf()
                .InSingletonScope();
            modKernel.ExposeService<IFishingApi, FishingApi>();
            modKernel.Bind<BaseFishProvider>()
                .ToSelf()
                .InSingletonScope();
            modKernel.ExposeService<IFishProvider, BaseFishProvider>();

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
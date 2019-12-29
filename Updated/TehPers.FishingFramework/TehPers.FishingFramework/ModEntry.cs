using System;
using System.Diagnostics.CodeAnalysis;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Config;
using TehPers.FishingFramework.Api.Events;
using TehPers.FishingFramework.Config;

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
            modKernel.Bind<ModInit>().ToSelf().InSingletonScope();
            modKernel.Bind<FishingOverrideService>().ToSelf().InSingletonScope();
            modKernel.AddEventHandler<FishingOverrideService>();
            modKernel.Bind<FishingApi>().ToSelf().InSingletonScope();
            modKernel.ExposeService<IFishingApi, FishingApi>();
            modKernel.BindConfiguration<IFishingConfiguration, FishingConfiguration>("config.json").InSingletonScope();
            modKernel.ExposeService<IFishingConfiguration>();
            // modKernel.Bind<EventHandler<FishCaughtEventArgs>>().ToConstant<EventHandler<FishCaughtEventArgs>>((sender, args) => { }).InSingletonScope();
        }
    }
}

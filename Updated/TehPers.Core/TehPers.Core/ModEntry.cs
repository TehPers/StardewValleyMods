using System.Diagnostics.CodeAnalysis;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Multiplayer;
using TehPers.Core.DependencyInjection.Lifecycle;
using TehPers.Core.Items;
using TehPers.Core.Modules;

namespace TehPers.Core
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Type is instantiated by SMAPI")]
    public class ModEntry : Mod, IServiceDrivenMod
    {
        private IModKernelFactory modKernelFactory;
        private LifecycleService lifecycleService;

        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log("Creating core API factory", LogLevel.Info);
            this.modKernelFactory = new ModKernelFactory();

            this.Register();
        }

        public void GameLoaded(IModKernel modKernel)
        {
            this.lifecycleService = modKernel.Get<LifecycleService>();
            this.lifecycleService.StartAll();
        }

        public void RegisterServices(IModKernel modKernel)
        {
            this.Monitor.Log("Registering mod services", LogLevel.Info);
            modKernel.Bind<LifecycleService>().ToSelf().InSingletonScope();

            this.Monitor.Log("Registering event managers", LogLevel.Info);
            modKernel.BindManagedSmapiEvents();

            this.Monitor.Log("Registering global services", LogLevel.Info);
            modKernel.Bind<EventChannelFactory>().ToSelf().InSingletonScope();
            modKernel.ExposeService<EventChannelFactory>();
            modKernel.GlobalKernel.Load(new GlobalServicesModule(modKernel));
        }

        public override object GetApi()
        {
            return this.modKernelFactory;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.modKernelFactory?.Dispose();
                this.lifecycleService?.StopAll();
            }

            base.Dispose(disposing);
        }
    }
}

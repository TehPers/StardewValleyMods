using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.DependencyInjection.Api;
using TehPers.Core.DependencyInjection.Api.Extensions;
using TehPers.Core.DependencyInjection.Modules;

namespace TehPers.Core.DependencyInjection
{
    public class ModEntry : Mod
    {
        private readonly IDependencyInjectionApi _diApi;

        public ModEntry()
        {
            this._diApi = new DependencyInjectionApi();
        }

        public override void Entry(IModHelper helper)
        {
            IModKernel modKernel = this._diApi.GetModKernel(this);
            this.RegisterServices(modKernel);
            this.Init(modKernel);
        }

        private void RegisterServices(IModKernel modKernel)
        {
            // Register dependencies
            modKernel.BindModTypes();
            modKernel.Global.Load(new GlobalModule(this, this._diApi));
            modKernel.Bind<ModInit>().ToSelf().InSingletonScope();
        }

        private void Init(IResolutionRoot modKernel)
        {
            this.Monitor.Log("Initializing mod");
            ModInit modInit = modKernel.Get<ModInit>();
            modInit.Init();
        }

        public override object GetApi()
        {
            return this._diApi;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                this._diApi?.Dispose();
            }
        }
    }
}

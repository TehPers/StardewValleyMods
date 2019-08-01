using Ninject.Modules;
using StardewModdingAPI;

namespace TehPers.Core.Modules
{
    internal class CoreModule : NinjectModule
    {
        private readonly IMod _mod;

        public CoreModule(IMod mod)
        {
            this._mod = mod;
        }

        public override void Load()
        {
            this.Bind<ModEntry>().ToSelf();
        }
    }
}
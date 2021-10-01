using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;

namespace TehPers.FishingOverhaul
{
    public class ModFishing : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var kernel = ModServices.Factory.GetKernel(this);
            kernel.Load<FishingModule>();

            var startup = kernel.Get<Startup>();
            startup.Initialize();
        }
    }
}
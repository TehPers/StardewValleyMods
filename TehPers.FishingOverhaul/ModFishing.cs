using System.Diagnostics.CodeAnalysis;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;

namespace TehPers.FishingOverhaul
{
    [SuppressMessage(
        "ReSharper",
        "UnusedMember.Global",
        Justification = "Class initialized and used by SMAPI."
    )]
    internal class ModFishing : Mod
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
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;

namespace TehPers.SpriteMain
{
    public class SpriteMainMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            if (ModServices.Factory is not { } kernelFactory)
            {
                this.Monitor.Log(
                    "Core mod seems to not be loaded. Aborting setup - this mod is effectively disabled.",
                    LogLevel.Error
                );

                return;
            }

            var kernel = kernelFactory.GetKernel(this);
            kernel.Load<SpriteMainModule>();
        }
    }
}
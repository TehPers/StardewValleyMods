using Ninject;
using Ninject.Extensions.Factory;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using TehPers.Core.DI;
using TehPers.Core.Modules;

namespace TehPers.Core
{
    public class ModCore : Mod
    {
        public ModCore()
        {
            // Create kernel factory and add core processors
            ModServices.Factory = new ModKernelFactory();
            ModServices.Factory.AddKernelProcessor(
                kernel =>
                {
                    kernel.Load(
                        new FuncModule(),
                        new ModServicesModule(kernel)
                    );
                }
            );
        }

        public override void Entry(IModHelper helper)
        {
            // Add processors
            ModServices.Factory.AddKernelProcessor(
                kernel => kernel.Load(new ModJsonModule(this.Helper, this.Monitor))
            );

            // Add 
            var kernel = ModServices.Factory.GetKernel(this);
            kernel.Load(new GlobalJsonModule(helper, this.Monitor));
            kernel.Load<NamespaceModule>();

            // Reload namespace registry on save loaded
            helper.Events.GameLoop.SaveLoaded +=
                (_, _) => kernel.Get<INamespaceRegistry>().Reload();
        }
    }
}
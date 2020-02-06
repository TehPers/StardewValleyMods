using Ninject;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Sample
{
    public class SampleMod : Mod, IServiceDrivenMod
    {
        public override void Entry(IModHelper helper)
        {
            this.Register();
        }

        public void RegisterServices(IModKernel modKernel)
        {
            // Add a simple event handler
            modKernel.BindSimpleEventHandler<ButtonPressedEventArgs>((sender, args) =>
            {
                if (args.Button == SButton.Y)
                {
                    Game1.showGlobalMessage("You pressed 'Y'!");
                }
            });

            // Load a module (collection of services)
            modKernel.Load(new SampleModule());

            // Bind an initializer
            modKernel.Bind<ModInitializer>().ToSelf().InSingletonScope();
        }

        public void GameLoaded(IModKernel modKernel)
        {
            // Perform some kind of initialization logic
            var initializer = modKernel.Get<ModInitializer>();
            initializer.Initialize();
        }
    }
}
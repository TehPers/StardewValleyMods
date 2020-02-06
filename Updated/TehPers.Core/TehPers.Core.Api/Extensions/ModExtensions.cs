using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>Extensions for <see cref="IMod"/> and <see cref="IServiceDrivenMod"/>.</summary>
    public static class ModExtensions
    {
        private const string CoreModId = "TehPers.Core";

        /// <summary>
        /// Asynchronously register services and initializes the mod.
        /// The services will be registered once all <see cref="Mod.Entry"/> methods have been executed, and the mod will be initialized after all mods have registered their services.
        /// This must be called in your mod's <see cref="IMod.Entry"/> method or it won't do anything.
        /// </summary>
        /// <param name="mod">The <see cref="IMod"/>.</param>
        /// <param name="registerServices">The callback which registers services to the <see cref="IModKernel"/> for this <see cref="IMod"/>.</param>
        /// <param name="initialize">The callback which initializes the mod, after all services have been loaded.</param>
        public static void Register(this IMod mod, Action<IModKernel> registerServices, Action<IModKernel> initialize)
        {
            _ = mod ?? throw new ArgumentNullException(nameof(mod));

            if (mod.ModManifest.UniqueID != ModExtensions.CoreModId && mod.ModManifest.Dependencies?.Any(dependency => dependency?.UniqueID == ModExtensions.CoreModId && dependency.IsRequired) != true)
            {
                throw new ArgumentException($"Mod must have '{ModExtensions.CoreModId}' listed as a required dependency in order to register services.", nameof(mod));
            }

            // Wait until next update tick
            mod.Helper.Events.GameLoop.GameLaunched += DoRegisterServices;

            void DoRegisterServices(object sender, GameLaunchedEventArgs gameLaunchedEventArgs)
            {
                mod.Helper.Events.GameLoop.GameLaunched -= DoRegisterServices;
                mod.Helper.Events.GameLoop.UpdateTicked += DoModInit;

                if (registerServices == null)
                {
                    return;
                }

                // Register services
                var modKernel = mod.Helper.ModRegistry.GetApi<IModKernelFactory>(ModExtensions.CoreModId).GetKernel(mod);
                registerServices(modKernel);
            }

            void DoModInit(object sender, UpdateTickedEventArgs args)
            {
                mod.Helper.Events.GameLoop.UpdateTicked -= DoModInit;

                if (initialize == null)
                {
                    return;
                }

                // Initialize mod
                var modKernel = mod.Helper.ModRegistry.GetApi<IModKernelFactory>(ModExtensions.CoreModId).GetKernel(mod);
                initialize(modKernel);
            }
        }

        /// <summary>
        /// Asynchronously registers services for this <see cref="IServiceDrivenMod"/> and initializes it.
        /// Services will be registered early during game initialization, and the mod will be initialized after all mods have had a chance to register services.
        /// This must be called in your mod's <see cref="IMod.Entry"/> method or it won't do anything.
        /// </summary>
        /// <param name="mod">The <see cref="IServiceDrivenMod"/>.</param>
        public static void Register(this IServiceDrivenMod mod)
        {
            _ = mod ?? throw new ArgumentNullException(nameof(mod));
            mod.Register(mod.RegisterServices, mod.GameLoaded);
        }
    }
}

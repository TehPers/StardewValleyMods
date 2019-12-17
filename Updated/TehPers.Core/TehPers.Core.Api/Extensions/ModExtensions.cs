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
        /// The services will be registered once all <see cref="Mod.Entry"/> methods have been executed, and the mod will be initailized after all mods have registered their services.
        /// </summary>
        /// <param name="mod">The <see cref="IMod"/>.</param>
        /// <param name="registerServices">The callback which registers services to the <see cref="IModKernel"/> for this <see cref="IMod"/>.</param>
        /// <param name="initialize">The callback which initializes the mod, after all services have been loaded.</param>
        public static void Register(this IMod mod, Action<IModKernel> registerServices, Action<IModKernel> initialize)
        {
            _ = mod ?? throw new ArgumentNullException(nameof(mod));
            _ = registerServices ?? throw new ArgumentNullException(nameof(registerServices));

            if (mod.ModManifest.UniqueID != CoreModId && !mod.ModManifest.Dependencies.Any(dependency => dependency.UniqueID == CoreModId && dependency.IsRequired))
            {
                throw new ArgumentException($"Mod must have '{CoreModId}' listed as a required dependency in order to register services.", nameof(mod));
            }

            // Wait until next update tick
            mod.Helper.Events.GameLoop.UpdateTicked += DoRegisterServices;

            void DoRegisterServices(object sender, UpdateTickedEventArgs args)
            {
                mod.Helper.Events.GameLoop.UpdateTicked -= DoRegisterServices;
                mod.Helper.Events.GameLoop.UpdateTicked += DoModInit;

                if (registerServices == null)
                {
                    return;
                }

                // Register services
                var modKernel = mod.Helper.ModRegistry.GetApi<IModKernelFactory>(ModExtensions.CoreModId).GetKernel(mod);
                registerServices.Invoke(modKernel);
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
                initialize.Invoke(modKernel);
            }
        }

        /// <summary>Asynchronously registers services for this <see cref="IServiceDrivenMod"/> and initializes it.</summary>
        /// <param name="mod">The <see cref="IServiceDrivenMod"/>.</param>
        public static void Register(this IServiceDrivenMod mod)
        {
            _ = mod ?? throw new ArgumentNullException(nameof(mod));
            mod.Register(mod.RegisterServices, mod.GameLoaded);
        }
    }
}

using StardewModdingAPI;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.Api
{
    /// <summary>A mod which is driven by services and relies on dependency injection.</summary>
    public interface IServiceDrivenMod : IMod
    {
        /// <summary>Registers this <see cref="IServiceDrivenMod"/>'s dependent services in the <see cref="IModKernel"/>.</summary>
        /// <param name="modKernel">The <see cref="IModKernel"/> for this <see cref="IMod"/>.</param>
        void RegisterServices(IModKernel modKernel);

        /// <summary>Initializes the mod after services have been registered.</summary>
        /// <param name="modKernel">The <see cref="IModKernel"/> for this <see cref="IMod"/>.</param>
        void GameLoaded(IModKernel modKernel);
    }
}

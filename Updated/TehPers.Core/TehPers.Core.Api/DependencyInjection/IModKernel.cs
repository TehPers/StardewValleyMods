using Ninject;
using StardewModdingAPI;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary><see cref="IKernel"/> specific to an <see cref="IMod"/>.</summary>
    public interface IModKernel : IKernel
    {
        /// <summary>
        /// Gets the mod which owns this <see cref="IModKernel"/>.
        /// </summary>
        IMod ParentMod { get; }

        /// <summary>
        /// Gets the global <see cref="IKernel"/>, which is the parent of this <see cref="IModKernel"/>. Dependencies registered in the global <see cref="IKernel"/> are visible to all mods.
        /// </summary>
        IKernel GlobalKernel { get; }

        /// <summary>
        /// Gets the <see cref="IModKernelFactory"/> that created this <see cref="IModKernel"/>.
        /// </summary>
        IModKernelFactory ParentFactory { get; }
    }
}
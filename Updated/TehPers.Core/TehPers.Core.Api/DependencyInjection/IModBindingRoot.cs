using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Provides a path for mods to register bindings.
    /// </summary>
    public interface IModBindingRoot : IBindingRoot, IProxyBindable
    {
        /// <summary>
        /// Gets the mod which owns this <see cref="IModBindingRoot"/>.
        /// </summary>
        IMod ParentMod { get; }

        /// <summary>
        /// Gets the <see cref="IModKernelFactory"/> that created this <see cref="IModBindingRoot"/>.
        /// </summary>
        IModKernelFactory ParentFactory { get; }
    }
}
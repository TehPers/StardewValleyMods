using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Provides a path for mods to register bindings.
    /// </summary>
    public interface IModBindingRoot : IBindingRoot
    {
        /// <summary>
        /// Gets the mod which owns this <see cref="IModBindingRoot"/>.
        /// </summary>
        IMod ParentMod { get; }

        /// <summary>
        /// Gets the global <see cref="IBindingRoot"/>, which is the parent of this <see cref="IModBindingRoot"/>. Dependencies registered in the global <see cref="IBindingRoot"/> are visible to all mods.
        /// </summary>
        IBindingRoot GlobalRoot { get; }

        /// <summary>
        /// Gets the <see cref="IModKernelFactory"/> that created this <see cref="IModBindingRoot"/>.
        /// </summary>
        IModKernelFactory ParentFactory { get; }
    }
}
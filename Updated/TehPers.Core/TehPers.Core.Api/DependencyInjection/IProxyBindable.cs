using Ninject.Syntax;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Indicates that an object has a proxy root for exposing services.
    /// </summary>
    public interface IProxyBindable
    {
        /// <summary>
        /// Gets an <see cref="IBindingRoot"/> which automatically creates proxy bindings in the global kernel when bindings are created.
        /// Dependencies registered in it are visible to all mods, however they are resolved by your <see cref="IModKernel"/>.
        /// </summary>
        IBindingRoot GlobalProxyRoot { get; }
    }
}
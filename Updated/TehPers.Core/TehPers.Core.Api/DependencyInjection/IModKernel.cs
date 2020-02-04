using Ninject;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// A factory capable of creating any type of object based on bindings.
    /// It is a child of the global kernel, so any missing bindings will be resolved by the global kernel.
    /// </summary>
    public interface IModKernel : IKernel, IModBindingRoot, IProxyBindable
    {
        /// <summary>
        /// Gets the global kernel. Services without bindings in this <see cref="IModKernel"/> are resolved by the global kernel.
        /// </summary>
        IKernel GlobalKernel { get; }
    }
}
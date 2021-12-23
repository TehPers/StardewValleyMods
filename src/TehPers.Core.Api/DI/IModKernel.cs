using Ninject;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// A factory capable of creating any type of object based on bindings.
    /// It is a child of the global kernel, so any missing bindings will be resolved by the global kernel.
    /// </summary>
    public interface IModKernel : IKernel, IModBindingRoot
    {
        /// <summary>
        /// Gets the global kernel. Services without bindings in this <see cref="IModKernel"/> are resolved by the global kernel.
        /// </summary>
        IGlobalKernel GlobalKernel { get; }
    }
}
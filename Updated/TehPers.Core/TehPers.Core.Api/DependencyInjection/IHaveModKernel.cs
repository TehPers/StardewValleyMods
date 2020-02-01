using Ninject.Infrastructure;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Indicates that an object has a reference to an <see cref="IModKernel"/>.
    /// </summary>
    public interface IHaveModKernel : IHaveKernel
    {
        /// <summary>
        /// Gets the kernel.
        /// </summary>
        new IModKernel Kernel { get; }
    }
}
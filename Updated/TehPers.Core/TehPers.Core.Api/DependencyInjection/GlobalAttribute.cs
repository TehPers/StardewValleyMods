using System;
using Ninject;
using Ninject.Planning.Bindings;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Forces a <see cref="IModKernel"/> to inherit a service's implementation from the global <see cref="IKernel"/>.
    /// </summary>
    public class GlobalAttribute : ConstraintAttribute
    {
        /// <inheritdoc />
        public override bool Matches(IBindingMetadata metadata)
        {
            // This is handled within the kernel
            return true;
        }
    }
}

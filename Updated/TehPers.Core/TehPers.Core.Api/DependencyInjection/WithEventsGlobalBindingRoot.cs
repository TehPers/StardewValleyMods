using System;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Provides a path to register bindings and automatically register them globally as event handlers.
    /// </summary>
    public class WithEventsGlobalBindingRoot : WithEventsBindingRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsGlobalBindingRoot"/> class.
        /// </summary>
        /// <param name="root">The binding root.</param>
        public WithEventsGlobalBindingRoot(IProxyBindable root)
            : base(root, root?.GlobalProxyRoot ?? throw new ArgumentNullException(nameof(root)))
        {
        }
    }
}
using System;
using Ninject.Syntax;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Provides a path to register bindings and automatically register them as event handlers.
    /// </summary>
    public class WithEventsModBindingRoot : WithEventsBindingRoot, IProxyBindable
    {
        private readonly IProxyBindable root;

        /// <inheritdoc />
        public IBindingRoot GlobalProxyRoot => new WithEventsGlobalBindingRoot(this.root);

        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsModBindingRoot"/> class.
        /// </summary>
        /// <param name="proxyRoot">The binding root.</param>
        public WithEventsModBindingRoot(IProxyBindable proxyRoot)
            : base(proxyRoot, proxyRoot)
        {
            this.root = proxyRoot ?? throw new ArgumentNullException(nameof(proxyRoot));
        }
    }
}
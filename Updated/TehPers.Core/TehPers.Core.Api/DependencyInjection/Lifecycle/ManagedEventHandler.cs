using System;

namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// Wrapper for lifecycle event handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of event handler being managed.</typeparam>
    public class ManagedEventHandler<THandler>
        where THandler : class
    {
        /// <summary>
        /// Gets the handler for the event.
        /// </summary>
        public THandler Handler { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedEventHandler{THandler}"/> class.
        /// </summary>
        /// <param name="handler">The handler for the managed event.</param>
        public ManagedEventHandler(THandler handler)
        {
            this.Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}

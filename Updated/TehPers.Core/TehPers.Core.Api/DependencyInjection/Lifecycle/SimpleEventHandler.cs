using System;

namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// A simple event handler that does not need any dependencies injected into it.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of arguments for the event.</typeparam>
    public class SimpleEventHandler<TEventArgs> : IEventHandler<TEventArgs>
        where TEventArgs : EventArgs
    {
        private readonly EventHandler<TEventArgs> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEventHandler{TEventArgs}"/> class.
        /// </summary>
        /// <param name="handler">The handler for the event.</param>
        public SimpleEventHandler(EventHandler<TEventArgs> handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <inheritdoc />
        public void HandleEvent(object sender, TEventArgs args)
        {
            this.handler(sender, args);
        }
    }
}

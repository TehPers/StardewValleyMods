using System;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;

namespace TehPers.Core.DependencyInjection.Lifecycle
{
    /// <summary>
    /// Manages an event by calling registered handlers as needed.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event args used by the event.</typeparam>
    internal abstract class EventManager<TEventArgs> : IEventManager
        where TEventArgs : EventArgs
    {
        private readonly ISimpleFactory<IEventHandler<TEventArgs>> handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManager{TEventArgs}"/> class.
        /// </summary>
        /// <param name="handlers">Gets the handlers for this managed event.</param>
        protected EventManager(ISimpleFactory<IEventHandler<TEventArgs>> handlers)
        {
            this.handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }

        /// <inheritdoc />
        public abstract void StartListening();

        /// <inheritdoc />
        public abstract void StopListening();

        /// <summary>
        /// Handles the managed event by notifying all subscribed event handlers.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event's args.</param>
        protected void HandleEvent(object sender, TEventArgs args)
        {
            foreach (var handler in this.handlers.GetAll())
            {
                handler.HandleEvent(sender, args);
            }
        }
    }
}
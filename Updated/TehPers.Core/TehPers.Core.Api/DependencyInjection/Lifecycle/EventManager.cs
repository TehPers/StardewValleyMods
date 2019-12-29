using System;

namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// Manages an event by calling registered handlers as needed.
    /// </summary>
    /// <typeparam name="THandler">The type of event handler being managed.</typeparam>
    /// <typeparam name="TEventArgs">The type of event args used by the event.</typeparam>
    public abstract class EventManager<THandler, TEventArgs> : IEventManager
        where THandler : class
    {
        private readonly ISimpleFactory<THandler> handlerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManager{THandler,TEventArgs}"/> class.
        /// </summary>
        /// <param name="handlerFactory">Gets the handlers for this managed event.</param>
        protected EventManager(ISimpleFactory<THandler> handlerFactory)
        {
            this.handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
        }

        /// <inheritdoc />
        public abstract void StartListening();

        /// <inheritdoc />
        public abstract void StopListening();

        /// <summary>
        /// Notifies a managed event handler that the event has occurred.
        /// </summary>
        /// <param name="handler">The managed event handler.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event's args.</param>
        protected abstract void NotifyHandler(THandler handler, object sender, TEventArgs eventArgs);

        /// <summary>
        /// Handles the managed event by notifying all subscribed event handlers.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event's args.</param>
        protected void HandleEvent(object sender, TEventArgs eventArgs)
        {
            foreach (var handler in this.handlerFactory.GetAll())
            {
                this.NotifyHandler(handler, sender, eventArgs);
            }
        }
    }
}
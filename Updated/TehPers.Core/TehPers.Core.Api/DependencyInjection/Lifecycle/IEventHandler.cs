using System;

namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// <p>A handler for lifecycle events (such as UpdateTicked or SaveLoaded), or for custom events created by other mods.</p>
    /// <br />
    /// <p>You don't need to implement <see cref="IEventHandler{TEventArgs}"/> explicitly for it to handle events, but it is the recommended way of implementing it.
    /// The event handler is most likely used by a separate service that doesn't care about your specific type, but instead only cares that it implements
    /// <see cref="IEventHandler{TEventArgs}"/>. Because of that, it is unlikely you want the event handler to be visible as part of your type's public API.</p>
    /// </summary>
    /// <typeparam name="TEventArgs">The type of arguments the event propagates.</typeparam>
    public interface IEventHandler<in TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Handles an invocation of the event.
        /// </summary>
        /// <param name="sender">The object that created the event.</param>
        /// <param name="args">The arguments for the event.</param>
        void HandleEvent(object sender, TEventArgs args);
    }
}

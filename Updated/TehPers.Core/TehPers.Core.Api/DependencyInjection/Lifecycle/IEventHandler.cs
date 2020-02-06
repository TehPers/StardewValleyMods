using System;

namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// A handler for lifecycle events (such as UpdateTicked or SaveLoaded), or for custom events created by other mods.<br />
    /// <br />
    /// It is recommended that you implement this interface explicitly rather than implicitly.
    /// The event handler is most likely used by a separate service that doesn't care about your specific type, but instead only cares that it implements
    /// <see cref="IEventHandler{TEventArgs}"/>. Because of that, it is unlikely you want the event handler to be visible as part of your type's public API.
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

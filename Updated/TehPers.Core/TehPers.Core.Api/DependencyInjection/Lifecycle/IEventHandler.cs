using System;

namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// A handler for lifecycle events (such as UpdateTicked or SaveLoaded), or for custom events created by other mods.<br />
    /// <br />
    /// It is recommended that you implement this interface explicitly for two reasons:<br />
    /// 1) The event handler is most likely used by a separate service that doesn't care about your specific type, but instead only cares that it implements <see cref="IEventHandler{TEventArgs}"/>. Because of that, it is unlikely you want the event handler to be visible as part of your type's public API.<br />
    /// 2) It is possible for a service to handle multiple events. Since the implementations would conflict, you would need to implement some of the handlers explicitly anyway.<br />
    /// You don't need to implement <see cref="IEventHandler{TEventArgs}"/> explicitly for it to handle events. However, keep point #1 in mind if you choose to implement the interface implicitly instead.
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

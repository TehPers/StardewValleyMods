namespace TehPers.Core.DependencyInjection.Lifecycle
{
    /// <summary>
    /// Manages an event by calling registered handlers as needed.
    /// </summary>
    internal interface IEventManager
    {
        /// <summary>
        /// Starts listening for events.
        /// </summary>
        void StartListening();

        /// <summary>
        /// Stops listening for events.
        /// </summary>
        void StopListening();
    }
}
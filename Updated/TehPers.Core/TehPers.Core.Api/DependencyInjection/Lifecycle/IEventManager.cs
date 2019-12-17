namespace TehPers.Core.Api.DependencyInjection.Lifecycle
{
    /// <summary>
    /// Manages an event by calling registered handlers as needed.
    /// </summary>
    public interface IEventManager
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
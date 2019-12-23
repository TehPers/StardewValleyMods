using StardewModdingAPI;

namespace TehPers.Core.Api.Multiplayer
{
    /// <summary>
    /// Factory for creating instances of <see cref="EventChannel{TMessage}"/>.
    /// </summary>
    public class EventChannelFactory
    {
        private readonly IModHelper coreHelper;
        private readonly IManifest coreManifest;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventChannelFactory"/> class.
        /// </summary>
        /// <param name="coreHelper">The core mod's helper.</param>
        /// <param name="coreManifest">The core mod's manifest.</param>
        public EventChannelFactory(IModHelper coreHelper, IManifest coreManifest)
        {
            this.coreHelper = coreHelper;
            this.coreManifest = coreManifest;
        }

        /// <summary>
        /// Creates an instance of <see cref="EventChannel{TMessage}"/> with the given ID.
        /// </summary>
        /// <typeparam name="T">The type of message being sent over the channel.</typeparam>
        /// <param name="id">The ID of the channel.</param>
        /// <returns>A new <see cref="EventChannel{TMessage}"/> with the given ID.</returns>
        public EventChannel<T> CreateEventChannel<T>(string id)
        {
            return new EventChannel<T>(this.coreManifest, this.coreHelper, id);
        }
    }
}
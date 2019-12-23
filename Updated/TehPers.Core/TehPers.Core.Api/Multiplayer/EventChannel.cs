using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TehPers.Core.Api.Multiplayer
{
    /// <summary>
    /// An event channel that is synchronized over multiplayer.
    /// </summary>
    /// <typeparam name="TMessage">The type of message being sent over the channel.</typeparam>
    public sealed class EventChannel<TMessage> : IDisposable
    {
        private const string MessageType = "eventChannel";

        private readonly IManifest coreManifest;
        private readonly IModHelper helper;

        /// <summary>
        /// Gets the ID of the channel.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Raised whenever a message is received on this channel.
        /// </summary>
        public event EventHandler<ChannelMessageReceivedEventArgs<TMessage>> MessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventChannel{TMessage}"/> class.
        /// </summary>
        /// <param name="coreManifest">The core mod's manifest.</param>
        /// <param name="helper">The mod's helper.</param>
        /// <param name="id">The ID of this event channel.</param>
        internal EventChannel(IManifest coreManifest, IModHelper helper, string id)
        {
            this.coreManifest = coreManifest ?? throw new ArgumentNullException(nameof(coreManifest));
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        /// Starts listening for multiplayer events.
        /// </summary>
        public void Listen()
        {
            this.helper.Events.Multiplayer.ModMessageReceived += this.MultiplayerOnModMessageReceived;
        }

        private void MultiplayerOnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type != EventChannel<TMessage>.MessageType || e.FromModID != this.coreManifest.UniqueID)
            {
                return;
            }

            var args = new ChannelMessageReceivedEventArgs<TMessage>(e.ReadAs<TMessage>(), e.FromModID, e.FromPlayerID);
            this.OnMessageReceived(args);
        }

        /// <summary>
        /// Sends a message over the channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="playerIds">The players to send the message to, or <see langword="null"/> to send it to all players.</param>
        public void Send(TMessage message, long[] playerIds = null)
        {
            this.helper.Multiplayer.SendMessage(message, EventChannel<TMessage>.MessageType, new[] { this.coreManifest.UniqueID }, playerIds);
        }

        private void OnMessageReceived(ChannelMessageReceivedEventArgs<TMessage> e)
        {
            this.MessageReceived?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.helper.Events.Multiplayer.ModMessageReceived -= this.MultiplayerOnModMessageReceived;
        }
    }
}

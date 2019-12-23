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
    /// Arguments for the event raised whenever an <see cref="EventChannel{TMessage}"/> receives a message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that was received.</typeparam>
    public class ChannelMessageReceivedEventArgs<TMessage>
    {
        /// <summary>
        /// Gets the message that was received.
        /// </summary>
        public TMessage Message { get; }

        /// <summary>
        /// Gets the unique ID of the mod that sent the message.
        /// </summary>
        public string SenderMod { get; }

        /// <summary>
        /// Gets the unique ID of the player that sent the message.
        /// </summary>
        public long SenderPlayer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelMessageReceivedEventArgs{TMessage}"/> class.
        /// </summary>
        /// <param name="message">The message that was received./</param>
        /// <param name="senderMod">The unique ID of the mod that sent the message.</param>
        /// <param name="senderPlayer">The unique ID of the player that sent the message.</param>
        public ChannelMessageReceivedEventArgs(TMessage message, string senderMod, long senderPlayer)
        {
            this.Message = message;
            this.SenderMod = senderMod;
            this.SenderPlayer = senderPlayer;
        }
    }
}

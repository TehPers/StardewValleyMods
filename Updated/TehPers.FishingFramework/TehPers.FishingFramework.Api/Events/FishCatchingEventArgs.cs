using System;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api;

namespace TehPers.FishingFramework.Api.Events
{
    /// <summary>
    /// Arguments for the event invoked right before the local player catches a fish.
    /// </summary>
    public class FishCatchingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the farmer that is catching the fish. This should be the local player.
        /// </summary>
        public Farmer Catcher { get; }

        /// <summary>
        /// Gets the fishing rod used to catch the fish.
        /// </summary>
        public FishingRod Rod { get; set; }

        /// <summary>
        /// Gets or sets the ID of the fish being caught.
        /// </summary>
        public NamespacedId FishId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FishCatchingEventArgs"/> class.
        /// </summary>
        /// <param name="catcher">The farmer that is catching the fish.</param>
        /// <param name="rod">The fishing rod used to catch the fish.</param>
        /// <param name="fishId">The ID of the fish being caught.</param>
        public FishCatchingEventArgs(Farmer catcher, FishingRod rod, NamespacedId fishId)
        {
            this.Catcher = catcher;
            this.Rod = rod;
            this.FishId = fishId;
        }
    }
}

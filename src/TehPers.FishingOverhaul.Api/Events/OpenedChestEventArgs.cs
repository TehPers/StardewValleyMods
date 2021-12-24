using StardewValley;
using System;
using System.Collections.Generic;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for whenever a treasure chest is opened.
    /// </summary>
    public class OpenedChestEventArgs : EventArgs
    {
        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The items in the chest.
        /// </summary>
        public IList<CaughtItem> CaughtItems { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedDefaultFishingInfoEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="caughtItems">The items in the chest.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public OpenedChestEventArgs(FishingInfo fishingInfo, IList<CaughtItem> caughtItems)
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.CaughtItems = caughtItems ?? throw new ArgumentNullException(nameof(caughtItems));
        }
    }
}

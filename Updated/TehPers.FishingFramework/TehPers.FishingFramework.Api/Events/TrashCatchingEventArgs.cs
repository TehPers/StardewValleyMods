using System;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api;
using SObject = StardewValley.Object;

namespace TehPers.FishingFramework.Api.Events
{
    /// <summary>
    /// Arguments for the event invoked whenever the local player catches trash, but before that trash is added to their inventory.
    /// </summary>
    public class TrashCatchingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the farmer that is catching the trash. This should be the local player.
        /// </summary>
        public Farmer Catcher { get; }

        /// <summary>
        /// Gets the fishing rod used to catch the trash.
        /// </summary>
        public FishingRod Rod { get; }

        /// <summary>
        /// Gets or sets the <see cref="Item.ParentSheetIndex"/> of the trash that is being caught. All trash must be instances of <see cref="SObject"/>.
        /// </summary>
        public NamespacedId TrashIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrashCatchingEventArgs"/> class.
        /// </summary>
        /// <param name="catcher">The farmer that is catching the trash.</param>
        /// <param name="rod">The fishing rod used to catch the trash.</param>
        /// <param name="trashIndex">The <see cref="Item.ParentSheetIndex"/> of the trash that is being caught.</param>
        public TrashCatchingEventArgs(Farmer catcher, FishingRod rod, NamespacedId trashIndex)
        {
            this.Catcher = catcher;
            this.Rod = rod;
            this.TrashIndex = trashIndex;
        }
    }
}
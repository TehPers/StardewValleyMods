using System;

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for when an item (either a fish or trash) is caught while fishing.
    /// </summary>
    public class CaughtItemEventArgs : EventArgs
    {
        /// <summary>
        /// The catch info.
        /// </summary>
        public CatchInfo Catch { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaughtItemEventArgs"/> class.
        /// </summary>
        /// <param name="catch">The catch info.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public CaughtItemEventArgs(CatchInfo @catch)
        {
            this.Catch = @catch ?? throw new ArgumentNullException(nameof(@catch));
        }
    }
}
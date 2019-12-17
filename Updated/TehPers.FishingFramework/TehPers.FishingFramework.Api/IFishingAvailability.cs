using StardewValley;
using TehPers.Core.Api.Chrono;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// An entity that is available sometimes while fishing.
    /// </summary>
    public interface IFishingAvailability
    {
        /// <summary>
        /// Gets the weighted chance that this item can be caught under certain circumstances. <br />
        /// <br />
        /// Keep in mind that some items may have no chance of appearing under those circumstances, so be sure to take that into account.
        /// The recommended way of checking if an item is available at all is to check if its weighted chance is greater than <c>1e-5d</c>.
        /// </summary>
        /// <param name="who">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="location">The <see cref="GameLocation"/> being fished in.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="water">The type of water being fished in.</param>
        /// <param name="dateTime">The current <see cref="SDateTime"/>.</param>
        /// <param name="mineLevel">The current mine level, or <see langword="null"/> if not in the mines.</param>
        /// <returns>The weighted chance that this item would be caught in the given circumstances.</returns>
        double GetWeightedChance(Farmer who, GameLocation location, Weathers weather, WaterType water, SDateTime dateTime, int? mineLevel = null);
    }
}
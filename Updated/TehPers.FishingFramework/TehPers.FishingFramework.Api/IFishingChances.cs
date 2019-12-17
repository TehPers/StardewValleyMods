using System.Collections.Generic;
using StardewValley;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// The chances of something occurring while fishing.<br />
    /// <br />
    /// Final chances are calculated as <c>(baseChance + dailyLuck * dailyLuckFactor + luckLevel * luckLevelFactor + streak * streakFactor) * locationFactor</c>.
    /// </summary>
    public interface IFishingChances
    {
        /// <summary>
        /// Gets or sets the base chance.
        /// </summary>
        double BaseChance { get; set; }

        /// <summary>
        /// Gets or sets the effect the perfect fishing streak has.
        /// </summary>
        double StreakFactor { get; set; }

        /// <summary>
        /// Gets or sets the effect that daily luck has.
        /// </summary>
        double DailyLuckFactor { get; set; }

        /// <summary>
        /// Gets or sets the effect that luck level has.
        /// </summary>
        double LuckLevelFactor { get; set; }

        /// <summary>
        /// Gets a mapping of locations to the effect those locations have on catching a fish. This is useful if you'd like to make it impossible to catch fish entirely in a location.
        /// </summary>
        IDictionary<GameLocation, double> LocationFactors { get; }
    }
}
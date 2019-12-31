using System.Collections.Generic;
using System.Dynamic;
using StardewValley;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// The chances of something occurring while fishing.<br />
    /// <br />
    /// Final chances are calculated as <c>(baseChance + dailyLuck * dailyLuckFactor + luckLevel * luckLevelFactor + streak * streakFactor) * locationFactor</c> bounded in the range [minChance, maxChance], then bounded once again in the range [0, 1].
    /// </summary>
    public interface IFishingChances
    {
        /// <summary>
        /// Gets the base chance.
        /// </summary>
        double BaseChance { get; }

        /// <summary>
        /// Gets the effect the perfect fishing streak has.
        /// </summary>
        double StreakFactor { get; }

        /// <summary>
        /// Gets the effect that fishing level has on this chance.
        /// </summary>
        double FishingLevelFactor { get; }

        /// <summary>
        /// Gets the effect that daily luck has.
        /// </summary>
        double DailyLuckFactor { get; }

        /// <summary>
        /// Gets the effect that luck level has.
        /// </summary>
        double LuckLevelFactor { get; }

        /// <summary>
        /// Gets a mapping of locations to the effect those locations have on this chance.
        /// </summary>
        IReadOnlyDictionary<GameLocation, double> LocationFactors { get; }

        /// <summary>
        /// Gets the minimum possible chance.
        /// </summary>
        double MinChance { get; }

        /// <summary>
        /// Gets the maximum possible chance.
        /// </summary>
        double MaxChance { get; }
    }

    /// <summary>
    /// The chances of finding treasure while fishing.
    /// </summary>
    public interface ITreasureChances : IFishingChances
    {
        /// <summary>
        /// Gets the effect that the magnet bait has.
        /// </summary>
        double MagnetFactor { get; }

        /// <summary>
        /// Gets the effect that the treasure hunter tackle has.
        /// </summary>
        double TreasureHunterFactor { get; }

        /// <summary>
        /// Gets the effect that the pirate profession has.
        /// </summary>
        double PirateFactor { get; }
    }
}
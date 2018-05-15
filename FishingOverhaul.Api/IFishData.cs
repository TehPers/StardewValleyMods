using StardewValley;
using TehPers.Core.Api.Enums;

namespace TehPers.FishingOverhaul.Api {
    public interface IFishData {
        /// <summary>Returns the weighted chance of this fish being selected (in comparison to other fish) for the given farmer.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>The weighted chance of this fish being selected.</returns>
        float GetWeight(Farmer who);

        /// <summary>Whether this fish meets the given criteria and can be caught.</summary>
        /// <param name="waterType">The type of water this fish is in.</param>
        /// <param name="season">The current season.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="time">The current time.</param>
        /// <param name="level">The current farmer's fishing level.</param>
        /// <param name="mineLevel">The current level in the mine, or null if not in the mine.</param>
        /// <returns>True if this fish can be caught, false otherwise.</returns>
        bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int level, int? mineLevel);
    }
}
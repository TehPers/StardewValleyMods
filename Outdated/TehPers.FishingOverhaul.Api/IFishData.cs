using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.FishingOverhaul.Api {
    public interface IFishData {
        /// <summary>Returns the weighted chance of this fish being selected (in comparison to other fish) for the given farmer.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>The weighted chance of this fish being selected.</returns>
        float GetWeight(Farmer who);

        /// <summary>Whether this fish meets the given criteria and can be caught.</summary>
        /// <param name="fish">The <see cref="Item.ParentSheetIndex"/> of the fish being caught.</param>
        /// <param name="waterTypes">The type of water this fish is in.</param>
        /// <param name="dateTime">The current date and time.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="level">The current farmer's fishing level.</param>
        /// <param name="mineLevel">The current level in the mine, or null if not in the mine.</param>
        /// <returns>True if this fish can be caught, false otherwise.</returns>
        bool MeetsCriteria(int fish, WaterTypes waterTypes, SDateTime dateTime, Weather weather, int level, int? mineLevel);
    }
}
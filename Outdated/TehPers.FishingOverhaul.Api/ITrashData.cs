using System.Collections.Generic;
using StardewValley;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Structs;
using TehPers.CoreMod.Api.Weighted;

namespace TehPers.FishingOverhaul.Api {
    public interface ITrashData : IWeighted {
        /// <summary>All the possible IDs this trash can be.</summary>
        IEnumerable<int> PossibleIds { get; }

        /// <summary>Whether this fish meets the given criteria and can be caught.</summary>
        /// <param name="who">The farmer that is fishing.</param>
        /// <param name="locationName">The name of the current location being fished in.</param>
        /// <param name="waterTypes">The type of water this fish is in.</param>
        /// <param name="dateTime">The current date and time.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="fishingLevel">The current farmer's fishing level.</param>
        /// <param name="mineLevel">The current level in the mine, or null if not in the mine.</param>
        /// <returns>True if this fish can be caught, false otherwise.</returns>
        bool MeetsCriteria(Farmer who, string locationName, WaterTypes waterTypes, SDateTime dateTime, Weather weather, int fishingLevel, int? mineLevel);
    }
}

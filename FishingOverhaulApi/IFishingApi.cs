using System.Collections.Generic;
using StardewValley;

namespace FishingOverhaul.Api {
    public interface IFishingApi {
        /// <summary>Gets the chance for a farmer to catch a fish.</summary>
        /// <param name="who">The farmer.</param>
        /// <returns>The chance for the given farmer to catch a fish.</returns>
        float GetFishChance(Farmer who);

        /// <summary>Sets the chance of catching fish. This overrides any fish chance calculations done by the mod itself.</summary>
        /// <param name="chance">The new chance of catching fish, or null to have the mod calculate fish chances.</param>
        void SetFishChance(float? chance);

        /// <summary>Gets all the fish data at the given location.</summary>
        /// <param name="location">The location to get fish data for.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing.</returns>
        IReadOnlyDictionary<int, IFishData> GetFishData(string location);

        /// <summary>Gets the fish data associated with a particular location and fish.</summary>
        /// <param name="location">The location to get fish data for.</param>
        /// <param name="fish">The ID of the fish to get data for.</param>
        /// <returns>The data associated with the given fish in the given location.</returns>
        IFishData GetFishData(string location, int fish);

        /// <summary>Sets the data for a fish at a specific location.</summary>
        /// <param name="location">The name of the location.</param>
        /// <param name="fish">The fish which the data is associated with.</param>
        /// <param name="data">The data to assign to the fish at the given location, or null if the data should be removed.</param>
        void SetFishData(string location, int fish, IFishData data);

        /// <summary>Removes all fish data overrides.</summary>
        void ResetFishData();

        /// <summary>Removes all fish data overrides at the given location.</summary>
        /// <param name="location">The location to remove overrides for.</param>
        /// <returns>True if overrides were removed, false if not.</returns>
        bool ResetFishData(string location);

        /// <summary>Removes the fish data override for the given fish at the given location if one exists.</summary>
        /// <param name="location">The location to remove the override for.</param>
        /// <param name="fish">The fish to remove the override for.</param>
        /// <returns>True if an override was removed, false if not.</returns>
        bool ResetFishData(string location, int fish);

        /// <summary>Adds new treasure data to the list of obtainable treasure.</summary>
        /// <param name="data">The data to add as a new treasure.</param>
        /// <returns>True if added, false if it's a duplicate entry.</returns>
        bool AddTreasureData(ITreasureData data);

        /// <summary>Removes existing treasure data from the list of obtainable treasure.</summary>
        /// <param name="data">The data that should be removed.</param>
        /// <returns>True if removed, false if the data doesn't exist.</returns>
        bool RemoveTreasureData(ITreasureData data);

        /// <summary>Gets all the obtainable treasure data.</summary>
        /// <returns>An <see cref="IEnumerable{ITreasureData}"/> containing all the available treasure data.</returns>
        IEnumerable<ITreasureData> GetTreasureData();
    }
}
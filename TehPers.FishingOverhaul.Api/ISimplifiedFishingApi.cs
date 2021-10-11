using System.Collections.Generic;
using StardewValley;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Simplified API for working with fishing. Prefer <see cref="IFishingApi"/> if possible.
    /// </summary>
    public interface ISimplifiedFishingApi
    {
        /// <summary>
        /// Gets the fish that can be caught. This does not take into account fish ponds.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="depth">The bobber depth.</param>
        /// <returns>The catchable fish as stringified namespaced keys.</returns>
        IEnumerable<string> GetCatchableFish(Farmer farmer, int depth = 4);

        /// <summary>
        /// Gets the trash that can be caught.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable trash as stringified namespaced keys.</returns>
        IEnumerable<string> GetCatchableTrash(Farmer farmer);

        /// <summary>
        /// Gets the treasure that can be caught.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable treasure as stringified namespaced keys.</returns>
        IEnumerable<string> GetCatchableTreasure(Farmer farmer);

        /// <summary>
        /// Gets the chance that a fish would be caught. This does not take into account whether
        /// there are actually fish to catch at the <see cref="Farmer"/>'s location. If no fish
        /// can be caught, then the <see cref="Farmer"/> will always catch trash.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> catching the fish.</param>
        /// <returns>The chance a fish would be caught instead of trash.</returns>
        double GetChanceForFish(Farmer farmer);

        /// <summary>
        /// Gets the chance that treasure will be found during the fishing minigame.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> catching the treasure.</param>
        /// <returns>The chance for treasure to appear during the fishing minigame.</returns>
        double GetChanceForTreasure(Farmer farmer);

        /// <summary>
        /// Gets whether a fish is legendary.
        /// </summary>
        /// <param name="fishKey">The item key of the fish as a stringified namespaced key.</param>
        /// <returns>Whether that fish is legendary.</returns>
        bool IsLegendary(string fishKey);

        /// <summary>
        /// Gets a <see cref="Farmer"/>'s current fishing streak.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> to get the streak of.</param>
        /// <returns>The <see cref="Farmer"/>'s streak.</returns>
        int GetStreak(Farmer farmer);

        /// <summary>
        /// Sets a <see cref="Farmer"/>'s current fishing streak.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> to set the streak of.</param>
        /// <param name="streak">The <see cref="Farmer"/>'s streak.</param>
        void SetStreak(Farmer farmer, int streak);

        /// <summary>
        /// Selects a random catch. A player may catch either a fish or trash item.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="bobberDepth">The bobber's water depth.</param>
        /// <param name="isFish">Whether the caught item is a fish.</param>
        /// <returns>A possible catch.</returns>
        string GetPossibleCatch(Farmer farmer, int bobberDepth, out bool isFish);

        /// <summary>
        /// Selects random treasure.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        IList<Item> GetPossibleTreasure(Farmer farmer);
    }
}
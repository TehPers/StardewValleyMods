using StardewValley;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Weighted;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Helper for working with fishing.
    /// </summary>
    public interface IFishingHelper
    {
        /// <summary>
        /// Gets the weighted chances of catching any fish.
        /// </summary>
        /// <param name="location">The location being fished in.</param>
        /// <param name="seasons">The seasons to get fish for.</param>
        /// <param name="weathers">The weathers to get fish for.</param>
        /// <param name="waterTypes">The water types to get fish for.</param>
        /// <param name="time">The time of day to get fish for.</param>
        /// <param name="fishingLevel">The <see cref="Farmer"/>'s fishing level.</param>
        /// <param name="dailyLuck">The <see cref="Farmer"/>'s daily luck.</param>
        /// <param name="depth">The bobber depth.</param>
        /// <returns>The catchable fish and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<NamespacedKey>> GetFishChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel,
            double dailyLuck,
            double depth = 4.0D
        );

        /// <summary>
        /// Gets the weighted chances of catching any fish.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="depth">The bobber depth.</param>
        /// <returns>The catchable fish and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<NamespacedKey>> GetFishChances(
            Farmer farmer,
            double depth = 4.0D
        )
        {
            var location = farmer.currentLocation;
            var season = location.GetSeasonForLocation() switch
            {
                "spring" => Seasons.Spring,
                "summer" => Seasons.Summer,
                "fall" => Seasons.Fall,
                "winter" => Seasons.Winter,
                _ => Seasons.None,
            };
            var weather = Game1.isRaining switch
            {
                true => Weathers.Rainy,
                false => Weathers.Sunny,
            };
            var waterType = location.getFishingLocation(farmer.getTileLocation()) switch
            {
                0 => WaterTypes.River,
                1 => WaterTypes.Pond,
                2 => WaterTypes.Freshwater,
                _ => WaterTypes.All,
            };

            return this.GetFishChances(
                location,
                season,
                weather,
                waterType,
                Game1.timeOfDay,
                farmer.FishingLevel,
                farmer.DailyLuck,
                depth
            );
        }

        /// <summary>
        /// Tries to get the traits for a fish.
        /// </summary>
        /// <param name="fishKey">The fish's <see cref="NamespacedKey"/>.</param>
        /// <param name="traits">The fish's traits.</param>
        /// <returns><see langword="true"/> if the fish has registered traits, otherwise <see langword="false"/>.</returns>
        bool TryGetFishTraits(NamespacedKey fishKey, [NotNullWhen(true)] out FishTraits? traits);

        /// <summary>
        /// Gets the weighted chances of catching any trash.
        /// </summary>
        /// <param name="location">The location being fished in.</param>
        /// <param name="seasons">The seasons to get trash for.</param>
        /// <param name="weathers">The weathers to get trash for.</param>
        /// <param name="waterTypes">The water types to get trash for.</param>
        /// <param name="time">The time of day to get trash for.</param>
        /// <param name="fishingLevel">The <see cref="Farmer"/>'s fishing level.</param>
        /// <returns>The catchable trash and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<NamespacedKey>> GetTrashChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel
        );

        /// <summary>
        /// Gets the weighted chances of catching any trash.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable trash and their chances of being caught.</returns>
        public IEnumerable<IWeightedValue<NamespacedKey>> GetTrashChances(
            Farmer farmer
        )
        {
            var location = farmer.currentLocation;
            var season = location.GetSeasonForLocation() switch
            {
                "spring" => Seasons.Spring,
                "summer" => Seasons.Summer,
                "fall" => Seasons.Fall,
                "winter" => Seasons.Winter,
                _ => Seasons.None,
            };
            var weather = Game1.isRaining switch
            {
                true => Weathers.Rainy,
                false => Weathers.Sunny,
            };
            var waterType = location.getFishingLocation(farmer.getTileLocation()) switch
            {
                0 => WaterTypes.River,
                1 => WaterTypes.Pond,
                2 => WaterTypes.Freshwater,
                _ => WaterTypes.All,
            };

            return this.GetTrashChances(location, season, weather, waterType, Game1.timeOfDay, farmer.FishingLevel);
        }

        /// <summary>
        /// Gets the chance that a fish would be caught. This does not take into account whether
        /// there are actually fish to catch at the farmer's location. If no fish can be caught,
        /// then the farmer will always catch trash.
        /// </summary>
        /// <param name="farmer">The farmer catching the fish.</param>
        /// <returns>The chance a fish would be caught instead of trash.</returns>
        double GetChanceForFish(Farmer farmer);

        /// <summary>
        /// Gets the chance that treasure will be found during the fishing minigame.
        /// </summary>
        /// <param name="farmer">The farmer catching the treasure.</param>
        /// <returns>The chance for treasure to appear during the fishing minigame.</returns>
        double GetChanceForTreasure(Farmer farmer);

        /// <summary>
        /// Gets whether a fish is legendary.
        /// </summary>
        /// <param name="fishKey">The item key of the fish.</param>
        /// <returns>Whether that fish is legendary.</returns>
        bool IsLegendary(NamespacedKey fishKey);

        /// <summary>
        /// Gets a <see cref="Farmer"/>'s current fishing streak.
        /// </summary>
        /// <param name="farmer">The farmer to get the streak of.</param>
        /// <returns>The farmer's streak.</returns>
        int GetStreak(Farmer farmer);

        /// <summary>
        /// Sets a <see cref="Farmer"/>'s current fishing streak.
        /// </summary>
        /// <param name="farmer">The farmer to set the streak of.</param>
        /// <param name="streak">The farmer's streak.</param>
        void SetStreak(Farmer farmer, int streak);
    }
}
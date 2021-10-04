using StardewValley;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Tools;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Weighted;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// API for working with fishing.
    /// </summary>
    public interface IFishingApi
    {
        /// <summary>
        /// Gets the weighted chances of catching any fish. This does not take into account fish
        /// ponds.
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
            int depth = 4
        );

        /// <summary>
        /// Gets the weighted chances of catching any fish. This does not take into account fish
        /// ponds.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="depth">The bobber depth.</param>
        /// <returns>The catchable fish and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<NamespacedKey>> GetFishChances(
            Farmer farmer,
            int depth = 4
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
                1 => WaterTypes.PondOrOcean,
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
        /// Gets the fish from a <see cref="FishPond"/> at the given tile if possible.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="bobberTile">The tile the bobber is on.</param>
        /// <param name="takeFish">If <see langword="false"/>, simulates taking the fish. Otherwise, actually pulls the fish from the pond.</param>
        /// <returns>The fish to get from the pond, if any.</returns>
        NamespacedKey? GetFishPondFish(
            Farmer farmer,
            Vector2 bobberTile,
            bool takeFish = false
        )
        {
            // Fish ponds are only on farms
            if (farmer.currentLocation is not Farm farm)
            {
                return null;
            }

            // Get the fish in that fish pond, if any
            return farm.buildings
                .OfType<FishPond>()
                .Where(pond => pond.isTileFishable(bobberTile))
                .Select(
                    pond =>
                    {
                        int? parentSheetIndex = takeFish switch
                        {
                            true when pond.CatchFish() is { ParentSheetIndex: var id } => id,
                            false when pond.currentOccupants.Value > 0 => pond.fishType.Value,
                            _ => null,
                        };
                        return parentSheetIndex.Map(NamespacedKey.SdvObject);
                    }
                )
                .FirstOrDefault();
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
        IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(
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
        public IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(
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
                1 => WaterTypes.PondOrOcean,
                2 => WaterTypes.Freshwater,
                _ => WaterTypes.All,
            };

            return this.GetTrashChances(location, season, weather, waterType, Game1.timeOfDay, farmer.FishingLevel);
        }

        /// <summary>
        /// Gets the weighted chances of catching any treasure.
        /// </summary>
        /// <param name="location">The location being fished in.</param>
        /// <param name="seasons">The seasons to get treasure for.</param>
        /// <param name="weathers">The weathers to get treasure for.</param>
        /// <param name="waterTypes">The water types to get treasure for.</param>
        /// <param name="time">The time of day to get treasure for.</param>
        /// <param name="fishingLevel">The <see cref="Farmer"/>'s fishing level.</param>
        /// <returns>The catchable treasure and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel
        );

        /// <summary>
        /// Gets the weighted chances of catching any treasure.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable treasure and their chances of being caught.</returns>
        public IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(
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
                1 => WaterTypes.PondOrOcean,
                2 => WaterTypes.Freshwater,
                _ => WaterTypes.All,
            };

            return this.GetTreasureChances(location, season, weather, waterType, Game1.timeOfDay, farmer.FishingLevel);
        }

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
        /// <param name="fishKey">The item key of the fish.</param>
        /// <returns>Whether that fish is legendary.</returns>
        bool IsLegendary(NamespacedKey fishKey);

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
        /// <param name="rod">The <see cref="FishingRod"/> used for fishing.</param>
        /// <param name="bobberDepth">The bobber's water depth.</param>
        /// <returns>A possible catch.</returns>
        PossibleCatch GetPossibleCatch(Farmer farmer, FishingRod rod, int bobberDepth);

        /// <summary>
        /// Selects random treasure.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that caught treasure.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        IList<Item> GetPossibleTreasure(Farmer farmer);

        /// <summary>
        /// Requests fishing data to be reloaded.
        /// </summary>
        void RequestReload();
    }
}
using StardewValley;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
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
    public interface IFishingApi : ISimplifiedFishingApi
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
        IEnumerable<IWeightedValue<NamespacedKey>> GetFishChances(Farmer farmer, int depth = 4)
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

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableFish(Farmer farmer, int depth)
        {
            return this.GetFishChances(farmer, depth)
                .Select(weightedValue => weightedValue.Value.ToString());
        }

        /// <summary>
        /// Gets the fish from a <see cref="FishPond"/> at the given tile if possible.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="bobberTile">The tile the bobber is on.</param>
        /// <param name="takeFish">If <see langword="false"/>, simulates taking the fish. Otherwise, actually pulls the fish from the pond.</param>
        /// <returns>The fish to get from the pond, if any.</returns>
        NamespacedKey? GetFishPondFish(Farmer farmer, Vector2 bobberTile, bool takeFish = false)
        {
            // Fish ponds are buildings
            if (farmer.currentLocation is not BuildableGameLocation buildableLocation)
            {
                return null;
            }

            // Get the fish in that fish pond, if any
            return buildableLocation.buildings.OfType<FishPond>()
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
        public IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(Farmer farmer)
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

            return this.GetTrashChances(
                location,
                season,
                weather,
                waterType,
                Game1.timeOfDay,
                farmer.FishingLevel
            );
        }

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableTrash(Farmer farmer)
        {
            return this.GetTrashChances(farmer)
                .Select(weightedValue => weightedValue.Value.ItemKey.ToString());
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
        public IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(Farmer farmer)
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

            return this.GetTreasureChances(
                location,
                season,
                weather,
                waterType,
                Game1.timeOfDay,
                farmer.FishingLevel
            );
        }

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableTreasure(Farmer farmer)
        {
            return this.GetTreasureChances(farmer)
                .SelectMany(weightedValue => weightedValue.Value.ItemKeys)
                .Select(key => key.ToString())
                .Distinct();
        }

        /// <summary>
        /// Tries to get the traits for a fish.
        /// </summary>
        /// <param name="fishKey">The fish's <see cref="NamespacedKey"/>.</param>
        /// <param name="traits">The fish's traits.</param>
        /// <returns><see langword="true"/> if the fish has registered traits, otherwise <see langword="false"/>.</returns>
        bool TryGetFishTraits(NamespacedKey fishKey, [NotNullWhen(true)] out FishTraits? traits);

        /// <summary>
        /// Gets whether a fish is legendary.
        /// </summary>
        /// <param name="fishKey">The item key of the fish.</param>
        /// <returns>Whether that fish is legendary.</returns>
        bool IsLegendary(NamespacedKey fishKey);

        bool ISimplifiedFishingApi.IsLegendary(string fishKey)
        {
            return NamespacedKey.TryParse(fishKey, out var key) && this.IsLegendary(key);
        }

        /// <summary>
        /// Selects a random catch. A player may catch either a fish or trash item.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="rod">The <see cref="FishingRod"/> used for fishing.</param>
        /// <param name="bobberDepth">The bobber's water depth.</param>
        /// <returns>A possible catch.</returns>
        PossibleCatch GetPossibleCatch(Farmer farmer, FishingRod rod, int bobberDepth);

        string ISimplifiedFishingApi.GetPossibleCatch(
            Farmer farmer,
            FishingRod rod,
            int bobberDepth,
            out bool isFish
        )
        {
            var (fishKey, catchType) = this.GetPossibleCatch(farmer, rod, bobberDepth);
            isFish = catchType is PossibleCatch.Type.Fish;
            return fishKey.ToString();
        }

        /// <summary>
        /// Requests fishing data to be reloaded.
        /// </summary>
        void RequestReload();
    }
}
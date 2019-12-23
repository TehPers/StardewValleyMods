using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using TehPers.Core.Api;
using TehPers.Core.Api.Chrono;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Weighted;

namespace TehPers.FishingFramework.Api.Extensions
{
    /// <summary>
    /// Extensions for the fishing API and its related types.
    /// </summary>
    public static class FishingApiExtensions
    {
        /// <summary>
        /// Checks if an item is a legendary fish.
        /// </summary>
        /// <param name="api">The fishing API.</param>
        /// <param name="fishId">The fish item's <see cref="NamespacedId"/>.</param>
        /// <returns><see langword="true"/> if the item is a legendary fish, <see langword="false"/> otherwise.</returns>
        public static bool IsLegendaryFish(this IFishingApi api, NamespacedId fishId)
        {
            _ = api ?? throw new ArgumentNullException(nameof(api));

            return api.FishTraits.TryGetValue(fishId, out var traits) && traits.IsLegendary;
        }

        /// <summary>
        /// Filters the items by whether they are currently available to the given <see cref="Farmer"/> and returns their weighted chances of being caught.
        /// </summary>
        /// <typeparam name="T">The type of items being filtered.</typeparam>
        /// <param name="items">The items being filtered.</param>
        /// <param name="who">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all the items that are currently available to be caught by the <see cref="Farmer"/> and returns their weighted chances of being caught.</returns>
        public static IEnumerable<IWeightedValue<T>> WhereAvailable<T>(this IEnumerable<T> items, Farmer who)
            where T : IFishingAvailability
        {
            _ = who ?? throw new ArgumentNullException(nameof(who));
            _ = items ?? throw new ArgumentNullException(nameof(items));

            if (!(who.currentLocation?.GetWaterType(who.getTileLocation()) is { } waterType))
            {
                waterType = WaterType.Any;
            }

            var mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : (int?)null;
            return items.WhereAvailable(who, who.currentLocation, Game1.isRaining ? Weathers.Rainy : Weathers.Sunny, waterType, SDateTime.Now, mineLevel);
        }

        /// <summary>
        /// Filters the items by whether they are available under certain circumstances and returns their weighted chances of being caught.
        /// </summary>
        /// <typeparam name="T">The type of items being filtered.</typeparam>
        /// <param name="items">The items being filtered.</param>
        /// <param name="who">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="location">The <see cref="GameLocation"/> being fished in.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="waterType">The type of water being fished in.</param>
        /// <param name="dateTime">The game's current date and time.</param>
        /// <param name="mineLevel">The current mine level, or <see langword="null" /> if not in the mines.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all of the items that are available to be caught in the given circumstances and returns their weighted chances of being caught.</returns>
        /// <remarks>This method is temporary.</remarks>
        public static IEnumerable<IWeightedValue<T>> WhereAvailable<T>(this IEnumerable<T> items, Farmer who, GameLocation location, Weathers weather, WaterType waterType, SDateTime dateTime, int? mineLevel = null)
            where T : IFishingAvailability
        {
            _ = location ?? throw new ArgumentNullException(nameof(location));
            _ = who ?? throw new ArgumentNullException(nameof(who));
            _ = items ?? throw new ArgumentNullException(nameof(items));

            return items
                .ToWeighted(item => item.GetWeightedChance(who, location, weather, waterType, dateTime, mineLevel))
                .Where(item => item.Weight > 1e-5d);
        }

        /// <summary>
        /// Gets the chance of a particular event for the given <see cref="Farmer"/>.
        /// </summary>
        /// <param name="chances">The chances of the event occurring.</param>
        /// <param name="who">The <see cref="Farmer"/>.</param>
        /// <param name="fishingStreak">The <see cref="Farmer"/>'s fishing streak.</param>
        /// <returns>The chance that the event occurs.</returns>
        public static double GetChance(this IFishingChances chances, Farmer who, int fishingStreak)
        {
            _ = who ?? throw new ArgumentNullException(nameof(who));
            _ = chances ?? throw new ArgumentNullException(nameof(chances));

            var locationFactor = chances.LocationFactors.TryGetValue(who.currentLocation, out var f) ? f : 1d;
            var normalChance = chances.BaseChance
                   + who.DailyLuck * chances.DailyLuckFactor
                   + who.LuckLevel * chances.LuckLevelFactor
                   + fishingStreak * chances.StreakFactor;

            return normalChance * locationFactor;
        }

        /// <summary>
        /// Gets the type of water at a particular tile in a <see cref="GameLocation"/>.
        /// </summary>
        /// <param name="location">The location the water is in.</param>
        /// <param name="tileLocation">The tile the water is at.</param>
        /// <returns>The type of water at that tile.</returns>
        public static WaterType GetWaterType(this GameLocation location, Vector2 tileLocation)
        {
            _ = location ?? throw new ArgumentNullException(nameof(location));

            return location.getFishingLocation(tileLocation) switch
            {
                -1 => WaterType.Any,
                0 => WaterType.River,
                1 => WaterType.Lake,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}

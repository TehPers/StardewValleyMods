﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using TehPers.Core.Api.Gameplay;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Information about a <see cref="Farmer"/> that is fishing.
    /// </summary>
    /// <param name="User">The <see cref="Farmer"/> that is fishing.</param>
    public record FishingInfo(Farmer User)
    {
        /// <summary>
        /// The times of day being fished at.
        /// </summary>
        public ImmutableArray<int> Times { get; init; } = ImmutableArray.Create(Game1.timeOfDay);

        /// <summary>
        /// The seasons being fished in.
        /// </summary>
        public Seasons Seasons { get; init; } = User.currentLocation.GetSeasonForLocation() switch
        {
            "spring" => Seasons.Spring,
            "summer" => Seasons.Summer,
            "fall" => Seasons.Fall,
            "winter" => Seasons.Winter,
            _ => Seasons.All,
        };

        /// <summary>
        /// The weathers being fished in.
        /// </summary>
        public Weathers Weathers { get; init; } = Game1.isRaining switch
        {
            true => Weathers.Rainy,
            false => Weathers.Sunny,
        };

        /// <summary>
        /// The water types being fished in.
        /// </summary>
        public WaterTypes WaterTypes { get; init; } =
            User.currentLocation.getFishingLocation(User.getTileLocation()) switch
            {
                0 => WaterTypes.River,
                1 => WaterTypes.PondOrOcean,
                2 => WaterTypes.Freshwater,
                _ => WaterTypes.All,
            };

        /// <summary>
        /// The fishing level of the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public int FishingLevel { get; init; } = User.FishingLevel;

        /// <summary>
        /// The bobber depth.
        /// </summary>
        public int BobberDepth { get; init; } = 4;

        /// <summary>
        /// The names of the locations being fished in.
        /// </summary>
        public ImmutableArray<string> Locations { get; init; } = FishingInfo
            .GetDefaultLocationNames(User.currentLocation)
            .ToImmutableArray();

        /// <summary>
        /// The fishing rod's bobber position.
        /// </summary>
        public Vector2 BobberPosition { get; init; } =
            User.CurrentTool is FishingRod { isFishing: true, bobber: { Value: var bobberPos } }
                ? bobberPos / 64
                : User.getStandingPosition() / 64;

        /// <summary>
        /// Gets the location names associated with a <see cref="GameLocation"/>. Some locations
        /// have names in addition to their normal names:
        ///
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="MineShaft"/></term>
        ///         <description>"UndergroundMine/#", where # is the floor number.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Farm"/></term>
        ///         <description>
        ///             "Farm/X", where X is one of "Standard", "Riverland", "Forest", "Hills",
        ///             "Wilderness", or "FourCorners". Only vanilla farms have this name.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="IslandLocation"/></term>
        ///         <description>"Island".</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="location">The location to get the names of.</param>
        /// <returns>The location's names.</returns>
        public static IEnumerable<string> GetDefaultLocationNames(GameLocation location)
        {
            return location switch
            {
                MineShaft { Name: { } name, mineLevel: var mineLevel } => new[]
                {
                    name, "UndergroundMine", $"UndergroundMine/{mineLevel}"
                },
                Farm { Name: { } name } => Game1.whichFarm switch
                {
                    0 => new[] { name, $"{name}/Standard" },
                    1 => new[] { name, $"{name}/Riverland" },
                    2 => new[] { name, $"{name}/Forest" },
                    3 => new[] { name, $"{name}/Hills" },
                    4 => new[] { name, $"{name}/Wilderness" },
                    5 => new[] { name, $"{name}/FourCorners" },
                    _ => new[] { name },
                },
                IslandLocation { Name: { } name } => new[] { name, "Island" },
                _ => new[] { location.Name },
            };
        }
    }
}
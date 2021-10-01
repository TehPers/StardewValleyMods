using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Api.Weighted;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Loading;

namespace TehPers.FishingOverhaul.Services
{
    /// <inheritdoc cref="IFishingHelper" />
    internal class FishingHelper : IFishingHelper
    {
        private static HashSet<NamespacedKey> LegendaryFish { get; } = new()
        {
            NamespacedKey.SdvObject(159),
            NamespacedKey.SdvObject(160),
            NamespacedKey.SdvObject(163),
            NamespacedKey.SdvObject(682),
            NamespacedKey.SdvObject(775),
        };

        private readonly FishingData fishingData;
        private readonly TrashData trashData;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;

        private readonly string stateKey;

        public FishingHelper(
            IManifest manifest,
            FishingData fishingData,
            TrashData trashData,
            FishConfig fishConfig,
            TreasureConfig treasureConfig
        )
        {
            this.fishingData = fishingData ?? throw new ArgumentNullException(nameof(fishingData));
            this.trashData = trashData ?? throw new ArgumentNullException(nameof(trashData));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig = treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));

            this.stateKey = $"{manifest.UniqueID}/fishing-state";
        }

        public IEnumerable<IWeightedValue<NamespacedKey>> GetFishChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel,
            double dailyLuck,
            double depth = 4.0D
        )
        {
            IEnumerable<IWeightedValue<NamespacedKey>> GetNormalFishChances()
            {
                if (!this.fishingData.FishAvailabilities.TryGetValue(location.Name, out var availabilities))
                {
                    return Enumerable.Empty<IWeightedValue<NamespacedKey>>();
                }

                return availabilities.SelectMany(
                        availability => availability.GetWeightedChance(
                                time,
                                seasons,
                                weathers,
                                depth,
                                fishingLevel,
                                waterTypes
                            )
                            .AsEnumerable()
                            .ToWeighted(weightedChance => weightedChance, _ => availability.FishKey)
                    )
                    .GroupBy(weightedValue => weightedValue.Value)
                    .ToWeighted(group => group.Sum(weightedValue => weightedValue.Weight), group => group.Key);
            }

            // Farms
            if (location.Name is "Farm")
            {
                return this.GetFarmFishChances(seasons, weathers, waterTypes, time, fishingLevel, dailyLuck, depth)
                    .Concat(
                        this.fishConfig.AllowFishOnAllFarms
                            ? GetNormalFishChances()
                            : Enumerable.Empty<IWeightedValue<NamespacedKey>>()
                    );
            }

            return GetNormalFishChances();
        }

        public bool TryGetFishTraits(NamespacedKey fishKey, [NotNullWhen(true)] out FishTraits? traits)
        {
            return this.fishingData.FishTraits.TryGetValue(fishKey, out traits);
        }

        public IEnumerable<IWeightedValue<NamespacedKey>> GetTrashChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel
        )
        {
            return this.trashData.Availabilities
                .SelectMany(
                    availability => availability.GetWeightedChance(time, seasons, weathers, fishingLevel, waterTypes)
                        .Map(weight => (key: availability.ItemKey, weight)).AsEnumerable()
                ).ToWeighted(entry => entry.weight, entry => entry.key);
        }

        private IEnumerable<IWeightedValue<NamespacedKey>> GetFarmFishChances(
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel,
            double dailyLuck,
            double depth = 4.0D
        )
        {
            IEnumerable<IWeightedValue<NamespacedKey>> GetLocationFish(string locationName) =>
                Game1.getLocationFromName(locationName) is { } location
                    ? this.GetFishChances(location, seasons, weathers, waterTypes, time, fishingLevel, dailyLuck, depth)
                    : Enumerable.Empty<IWeightedValue<NamespacedKey>>();

            return Game1.whichFarm switch
            {
                // Standard: default farm fish
                0 => Enumerable.Empty<IWeightedValue<NamespacedKey>>(),
                // Riverland: forest + town + default farm fish
                1 => GetLocationFish("Forest")
                    .Concat(GetLocationFish("Town")),
                // Forest: forest + woodskip + default farm fish
                2 => GetLocationFish("Forest")
                    .Append(new WeightedValue<NamespacedKey>(NamespacedKey.SdvObject(734), 0.05 + dailyLuck)),
                // Hills: forest + default farm fish
                3 => GetLocationFish("Forest"),
                // Wilderness: mountain + default farm fish
                4 => GetLocationFish("Mountain"),
                // Four corners: forest + mountain + default farm fish
                5 => GetLocationFish("Forest")
                    .Concat(GetLocationFish("Mountain")),
                _ => Enumerable.Empty<IWeightedValue<NamespacedKey>>()
            };
        }

        public double GetChanceForFish(Farmer farmer)
        {
            var streak = this.GetStreak(farmer);
            return this.fishConfig.FishChances.GetChance(farmer, streak);
        }

        public double GetChanceForTreasure(Farmer farmer)
        {
            var streak = this.GetStreak(farmer);
            return this.treasureConfig.TreasureChances.GetChance(farmer, streak);
        }

        public bool IsLegendary(NamespacedKey fishKey)
        {
            return FishingHelper.LegendaryFish.Contains(fishKey);
        }

        public int GetStreak(Farmer farmer)
        {
            var key = $"{this.stateKey}/streak";
            return farmer.modData.TryGetValue(key, out var rawData) && int.TryParse(rawData, out var streak)
                ? streak
                : 0;
        }

        public void SetStreak(Farmer farmer, int streak)
        {
            var key = $"{this.stateKey}/streak";
            farmer.modData[key] = streak.ToString();
        }
    }
}
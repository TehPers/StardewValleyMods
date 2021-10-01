using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Loading;

namespace TehPers.FishingOverhaul.Setup
{
    public sealed class FishLoader : ISetup, IDisposable
    {
        private readonly IAssetProvider assetProvider;
        private readonly FishingData fishingData;
        private readonly INamespaceRegistry namespaceRegistry;

        public FishLoader(
            [ContentSource(ContentSource.GameContent)]
            IAssetProvider assetProvider,
            FishingData fishingData,
            INamespaceRegistry namespaceRegistry
        )
        {
            this.assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            this.fishingData = fishingData ?? throw new ArgumentNullException(nameof(fishingData));
            this.namespaceRegistry = namespaceRegistry ?? throw new ArgumentNullException(nameof(namespaceRegistry));
        }

        public void Setup()
        {
            this.namespaceRegistry.OnReload += this.ReloadDefaultFishData;
        }

        public void Dispose()
        {
            this.namespaceRegistry.OnReload -= this.ReloadDefaultFishData;
        }

        private void ReloadDefaultFishData(object? sender, EventArgs e)
        {
            this.fishingData.FishAvailabilities.Clear();
            this.fishingData.FishTraits.Clear();

            // Parse the fish traits
            var fish = this.assetProvider.Load<Dictionary<int, string>>(@"Data\Fish.xnb");
            var partialAvailabilities = new Dictionary<int, List<FishAvailability>>(fish.Count);
            foreach (var (fishId, rawFishInfo) in fish)
            {
                var fishInfo = rawFishInfo.Split('/');

                // TODO: maybe do something with fish caught in pots

                // TODO: check if this is needed
                if (fishInfo[1] == "5")
                {
                    continue;
                }

                // Parse fields
                if (!FishLoader.TryParseFishInfo(fishId, fishInfo, out var traits, out var availabilities))
                {
                    continue;
                }

                var fishKey = NamespacedKey.SdvObject(fishId);
                this.fishingData.FishTraits[fishKey] = traits;
                partialAvailabilities[fishId] = availabilities;
            }

            // Parse the location data
            var locations =
                this.assetProvider.Load<Dictionary<string, string>>(@"Data\Locations.xnb");
            foreach (var (locationName, rawLocationData) in locations)
            {
                var locationData = rawLocationData.Split('/');
                const int offset = 4;

                // Parse each season's data
                this.fishingData.FishAvailabilities[locationName] = new List<FishAvailability>();
                var seasons = Seasons.None;
                foreach (var seasonData in locationData.Skip(offset).Take(4).Select(data => data.Split(' ')))
                {
                    // Cycle season
                    seasons = seasons switch
                    {
                        Seasons.None => Seasons.Spring,
                        Seasons.Spring => Seasons.Summer,
                        Seasons.Summer => Seasons.Fall,
                        Seasons.Fall => Seasons.Winter,
                        _ => Seasons.None
                    };

                    // Check if too many iterations
                    if (seasons == Seasons.None)
                    {
                        break;
                    }

                    // Parse each fish's data
                    for (var i = 0; i < seasonData.Length - 1; i += 2)
                    {
                        // Fish ID
                        if (!int.TryParse(seasonData[i], out var fishId))
                        {
                            continue;
                        }

                        // Water type
                        if (!int.TryParse(seasonData[i + 1], out var waterTypeId))
                        {
                            continue;
                        }

                        var waterTypes = waterTypeId switch
                        {
                            -1 => WaterTypes.All,
                            0 => WaterTypes.River,
                            1 => WaterTypes.Pond,
                            2 => WaterTypes.Freshwater,
                            _ => WaterTypes.All,
                        };

                        // Add availabilities
                        if (!partialAvailabilities.TryGetValue(fishId, out var availabilities))
                        {
                            continue;
                        }

                        this.fishingData.FishAvailabilities[locationName].AddRange(
                            availabilities.Select(
                                availability => new FishAvailability(
                                    fishKey: availability.FishKey,
                                    startTime: availability.StartTime,
                                    endTime: availability.EndTime,
                                    seasons: seasons,
                                    weathers: availability.Weathers,
                                    waterTypes: waterTypes,
                                    maxDepth: availability.MaxDepth,
                                    spawnMultiplier: availability.SpawnMultiplier,
                                    depthMultiplier: availability.DepthMultiplier,
                                    minFishingLevel: availability.MinFishingLevel
                                )
                            )
                        );
                    }
                }
            }
        }

        private static bool TryParseFishInfo(
            int fishId,
            IReadOnlyList<string> fishInfo,
            [NotNullWhen(true)] out FishTraits? traits,
            [NotNullWhen(true)] out List<FishAvailability>? partialAvailability
        )
        {
            traits = default;
            partialAvailability = default;
            if (fishInfo.Count < 13)
            {
                return false;
            }

            // Dart frequency
            if (!int.TryParse(fishInfo[1], out var dartFrequency))
            {
                return false;
            }

            // Dart behavior
            var dartBehavior = fishInfo[2];

            // Min size
            if (!int.TryParse(fishInfo[3], out var minSize))
            {
                return false;
            }

            // Max size
            if (!int.TryParse(fishInfo[4], out var maxSize))
            {
                return false;
            }

            // Weathers
            var weathers = fishInfo[7].Split(' ').Aggregate(
                Weathers.None,
                (weathers, cur) =>
                {
                    return weathers
                        | cur switch
                        {
                            "sunny" => Weathers.Sunny, "rainy" => Weathers.Rainy, "both" => Weathers.All,
                            _ => Weathers.None,
                        };
                }
            );

            // Max depth
            if (!int.TryParse(fishInfo[9], out var maxDepth))
            {
                return false;
            }

            // Spawn multiplier
            if (!float.TryParse(fishInfo[10], out var spawnMultiplier))
            {
                return false;
            }

            // Depth multiplier
            if (!float.TryParse(fishInfo[11], out var depthMultiplier))
            {
                return false;
            }

            // Min fishing level
            if (!int.TryParse(fishInfo[12], out var minFishingLevel))
            {
                return false;
            }

            // Parse times and populate spawn availabilities
            var times = fishInfo[5].Split(' ');
            partialAvailability = new List<FishAvailability>(times.Length / 2);
            for (var i = 0; i < times.Length - 1; i += 2)
            {
                // Start time
                if (!int.TryParse(times[i], out var startTime))
                {
                    continue;
                }

                // End time
                if (!int.TryParse(times[i + 1], out var endTime))
                {
                    continue;
                }

                partialAvailability.Add(
                    new FishAvailability(
                        fishKey: NamespacedKey.SdvObject(fishId),
                        startTime: startTime,
                        endTime: endTime,
                        weathers: weathers,
                        maxDepth: maxDepth,
                        spawnMultiplier: spawnMultiplier,
                        depthMultiplier: depthMultiplier,
                        minFishingLevel: minFishingLevel
                    )
                );
            }

            // Set traits
            traits = new FishTraits(dartFrequency, dartBehavior, minSize, maxSize);

            return true;
        }
    }
}
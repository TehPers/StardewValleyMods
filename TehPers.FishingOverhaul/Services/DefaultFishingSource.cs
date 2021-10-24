using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource : IFishingContentSource
    {
        // Legendary fish
        private static readonly NamespacedKey crimsonfishKey = NamespacedKey.SdvObject(159);
        private static readonly NamespacedKey anglerKey = NamespacedKey.SdvObject(160);
        private static readonly NamespacedKey legendKey = NamespacedKey.SdvObject(163);
        private static readonly NamespacedKey mutantCarpKey = NamespacedKey.SdvObject(682);
        private static readonly NamespacedKey glacierfishKey = NamespacedKey.SdvObject(775);

        // Legendary II fish
        private static readonly NamespacedKey sonOfCrimsonfishKey = NamespacedKey.SdvObject(898);
        private static readonly NamespacedKey msAnglerKey = NamespacedKey.SdvObject(899);
        private static readonly NamespacedKey legend2Key = NamespacedKey.SdvObject(900);
        private static readonly NamespacedKey radioactiveCarpKey = NamespacedKey.SdvObject(901);
        private static readonly NamespacedKey glacierfishJrKey = NamespacedKey.SdvObject(992);

        private static readonly HashSet<NamespacedKey> vanillaLegendaries = new()
        {
            DefaultFishingSource.crimsonfishKey,
            DefaultFishingSource.anglerKey,
            DefaultFishingSource.legendKey,
            DefaultFishingSource.mutantCarpKey,
            DefaultFishingSource.glacierfishKey
        };

        private readonly IManifest manifest;
        private readonly IAssetProvider assetProvider;

        private readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        private readonly List<FishEntry> fishEntries;
        private readonly List<TrashEntry> trashEntries;
        private readonly List<TreasureEntry> treasureEntries;

        public event EventHandler? ReloadRequested;

        public DefaultFishingSource(
            IManifest manifest,
            [ContentSource(ContentSource.GameContent)] IAssetProvider assetProvider
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.assetProvider =
                assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));

            this.fishTraits = new();
            this.fishEntries = new();
            this.trashEntries = this.GetDefaultTrashData();
            this.treasureEntries = this.GetDefaultTreasureData();
        }

        public IEnumerable<FishingContent> Reload()
        {
            this.ReloadDefaultFishData();

            yield return new(this.manifest)
            {
                SetFishTraits = this.fishTraits.ToImmutableDictionary(),
                AddFish = this.fishEntries.ToImmutableArray(),
                AddTrash = this.trashEntries.ToImmutableArray(),
                AddTreasure = this.treasureEntries.ToImmutableArray(),
            };
        }

        private void ReloadDefaultFishData()
        {
            this.fishEntries.Clear();
            this.fishTraits.Clear();

            // Parse the fish traits
            var fish = this.assetProvider.Load<Dictionary<int, string>>(@"Data\Fish.xnb");
            var partialAvailabilities = new Dictionary<int, List<FishAvailabilityInfo>>(fish.Count);
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
                if (!DefaultFishingSource.TryParseFishInfo(
                    fishInfo,
                    out var traits,
                    out var availabilities
                ))
                {
                    continue;
                }

                var fishKey = NamespacedKey.SdvObject(fishId);
                this.fishTraits[fishKey] = traits with
                {
                    IsLegendary = DefaultFishingSource.vanillaLegendaries.Contains(fishKey),
                };
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
                var seasons = Seasons.None;
                foreach (var seasonData in locationData.Skip(offset)
                    .Take(4)
                    .Select(data => data.Split(' ')))
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
                            1 => WaterTypes.PondOrOcean,
                            2 => WaterTypes.Freshwater,
                            _ => WaterTypes.All,
                        };

                        // Add availabilities
                        if (!partialAvailabilities.TryGetValue(fishId, out var availabilities))
                        {
                            continue;
                        }

                        // Add all the fish entries
                        this.fishEntries.AddRange(
                            availabilities.Select(
                                availability => new FishEntry(
                                    NamespacedKey.SdvObject(fishId),
                                    availability with
                                    {
                                        Seasons = seasons,
                                        WaterTypes = waterTypes,
                                        IncludeLocations = locationName switch
                                        {
                                            // Include both beach locations
                                            "Beach" => ImmutableArray.Create(
                                                locationName,
                                                "BeachNightMarket"
                                            ),
                                            // Many farms use forest fish
                                            "Forest" => ImmutableArray.Create(
                                                locationName,
                                                "Farm/Riverland",
                                                "Farm/Forest",
                                                "Farm/Hills",
                                                "Farm/FourCorners"
                                            ),
                                            // Riverland farm uses town fish
                                            "Town" => ImmutableArray.Create(
                                                locationName,
                                                "Farm/Riverland"
                                            ),
                                            // Some farms use mountain fish
                                            "Mountain" => ImmutableArray.Create(
                                                locationName,
                                                "Farm/Mountain",
                                                "Farm/FourCorners"
                                            ),
                                            // Normal handling
                                            _ => ImmutableArray.Create(locationName),
                                        },
                                    }
                                )
                            )
                        );
                    }
                }
            }

            // Special entries
            var hasCaughtFish = new Dictionary<string, string>
            {
                ["HasValue:{{HasCaughtFish}}"] = "true",
            }.ToImmutableDictionary();
            this.fishEntries.AddRange(
                new FishEntry[]
                {
                    // Legendary fish
                    new(
                        DefaultFishingSource.crimsonfishKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Summer,
                            MinFishingLevel = 5,
                            IncludeLocations = ImmutableArray.Create("Beach", "BeachNightMarket"),
                            When = hasCaughtFish,
                        }
                    ),
                    new(
                        DefaultFishingSource.sonOfCrimsonfishKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Summer,
                            MinFishingLevel = 5,
                            IncludeLocations = ImmutableArray.Create("Beach", "BeachNightMarket"),
                            When = hasCaughtFish.Add(
                                    "TehPers.FishingOverhaul/SpecialOrderRuleActive",
                                    "LEGENDARY_FAMILY"
                                )
                                .Add("HasMod", "TehPers.FishingOverhaul"),
                        }
                    ),
                    new(
                        DefaultFishingSource.anglerKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Fall,
                            MinFishingLevel = 3,
                            IncludeLocations = ImmutableArray.Create("Town"),
                            When = hasCaughtFish,
                        }
                    ),
                    new(
                        DefaultFishingSource.msAnglerKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Fall,
                            MinFishingLevel = 3,
                            IncludeLocations = ImmutableArray.Create("Town"),
                            When = hasCaughtFish.Add(
                                    "TehPers.FishingOverhaul/SpecialOrderRuleActive",
                                    "LEGENDARY_FAMILY"
                                )
                                .Add("HasMod", "TehPers.FishingOverhaul"),
                        }
                    ),
                    new(
                        DefaultFishingSource.legendKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Spring,
                            Weathers = Weathers.Rainy,
                            WaterTypes = WaterTypes.PondOrOcean,
                            MinFishingLevel = 10,
                            IncludeLocations = ImmutableArray.Create("Mountain"),
                            When = hasCaughtFish,
                        }
                    ),
                    new(
                        DefaultFishingSource.legend2Key,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Spring,
                            Weathers = Weathers.Rainy,
                            WaterTypes = WaterTypes.PondOrOcean,
                            MinFishingLevel = 10,
                            IncludeLocations = ImmutableArray.Create("Mountain"),
                            When = hasCaughtFish.Add(
                                    "TehPers.FishingOverhaul/SpecialOrderRuleActive",
                                    "LEGENDARY_FAMILY"
                                )
                                .Add("HasMod", "TehPers.FishingOverhaul"),
                        }
                    ),
                    new(
                        DefaultFishingSource.mutantCarpKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            IncludeLocations = ImmutableArray.Create("Sewer"),
                            When = hasCaughtFish,
                        }
                    ),
                    new(
                        DefaultFishingSource.radioactiveCarpKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            IncludeLocations = ImmutableArray.Create("Sewer"),
                            When = hasCaughtFish.Add(
                                    "TehPers.FishingOverhaul/SpecialOrderRuleActive",
                                    "LEGENDARY_FAMILY"
                                )
                                .Add("HasMod", "TehPers.FishingOverhaul"),
                        }
                    ),
                    new(
                        DefaultFishingSource.glacierfishKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Winter,
                            WaterTypes = WaterTypes.River,
                            MinFishingLevel = 6,
                            IncludeLocations = ImmutableArray.Create("Forest"),
                            When = hasCaughtFish,
                        }
                    ),
                    new(
                        DefaultFishingSource.glacierfishJrKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            Seasons = Seasons.Winter,
                            WaterTypes = WaterTypes.River,
                            MinFishingLevel = 6,
                            IncludeLocations = ImmutableArray.Create("Forest"),
                            When = hasCaughtFish.Add(
                                    "TehPers.FishingOverhaul/SpecialOrderRuleActive",
                                    "LEGENDARY_FAMILY"
                                )
                                .Add("HasMod", "TehPers.FishingOverhaul"),
                        }
                    ),

                    // Forest farm
                    new(
                        NamespacedKey.SdvObject(734),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = ImmutableArray.Create("Farm/Forest"),
                        }
                    ),

                    // UndergroundMine
                    new(
                        NamespacedKey.SdvObject(158),
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            IncludeLocations = ImmutableArray.Create(
                                "UndergroundMine/0",
                                "UndergroundMine/10"
                            ),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(161),
                        new(0.015)
                        {
                            DepthMultiplier = 0.015 / 4,
                            IncludeLocations = ImmutableArray.Create("UndergroundMine/40"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(162),
                        new(0.01)
                        {
                            DepthMultiplier = 0.01 / 4,
                            IncludeLocations = ImmutableArray.Create("UndergroundMine/80"),
                        }
                    ),

                    // Submarine
                    new(
                        NamespacedKey.SdvObject(149),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(154),
                        new(0.08)
                        {
                            DepthMultiplier = 0.08 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(155),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(798),
                        new(0.28)
                        {
                            DepthMultiplier = 0.28 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(799),
                        new(0.18)
                        {
                            DepthMultiplier = 0.18 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(800),
                        new(0.1)
                        {
                            DepthMultiplier = 0.1 / 4,
                            IncludeLocations = ImmutableArray.Create("Submarine"),
                        }
                    ),

                    // Volcano caldera
                    new(
                        NamespacedKey.SdvObject(162),
                        new(0.1)
                        {
                            DepthMultiplier = 0.1 / 4,
                            IncludeLocations = ImmutableArray.Create("Caldera"),
                        }
                    )
                }
            );

            // Request reload
            this.ReloadRequested?.Invoke(this, EventArgs.Empty);
        }

        private static bool TryParseFishInfo(
            IReadOnlyList<string> fishInfo,
            [NotNullWhen(true)] out FishTraits? traits,
            [NotNullWhen(true)] out List<FishAvailabilityInfo>? partialAvailability
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
            var dartBehavior = fishInfo[2] switch
            {
                "mixed" => DartBehavior.Mixed,
                "dart" => DartBehavior.Dart,
                "smooth" => DartBehavior.Smooth,
                "sink" => DartBehavior.Sink,
                "floater" => DartBehavior.Floater,
                _ => DartBehavior.Mixed,
            };

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
            var weathers = fishInfo[7]
                .Split(' ')
                .Aggregate(
                    Weathers.None,
                    (weathers, cur) =>
                    {
                        return weathers
                            | cur switch
                            {
                                "sunny" => Weathers.Sunny,
                                "rainy" => Weathers.Rainy,
                                "both" => Weathers.All,
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
            if (!float.TryParse(fishInfo[10], out var weightedChance))
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
            partialAvailability = new(times.Length / 2);
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
                    new(weightedChance)
                    {
                        DepthMultiplier = depthMultiplier,
                        MaxDepth = maxDepth,
                        StartTime = startTime,
                        EndTime = endTime,
                        Weathers = weathers,
                        MinFishingLevel = minFishingLevel,
                    }
                );
            }

            // Set traits
            traits = new(dartFrequency, dartBehavior, minSize, maxSize);

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed class DefaultFishingSource : IFishingContentSource
    {
        private static readonly Seasons[] formatSeasons =
        {
            Seasons.Spring, Seasons.Summer, Seasons.Fall, Seasons.Winter
        };

        private static readonly Weathers[] formatWeathers = { Weathers.Sunny, Weathers.Rainy };

        private readonly IManifest manifest;
        private readonly IMonitor monitor;
        private readonly IAssetProvider assetProvider;

        private readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        private readonly List<FishEntry> fishEntries;
        private readonly List<TrashEntry> trashEntries;
        private readonly List<TreasureEntry> treasureEntries;

        public event EventHandler? ReloadRequested;

        public DefaultFishingSource(
            IManifest manifest,
            IMonitor monitor,
            [ContentSource(ContentSource.GameContent)] IAssetProvider assetProvider
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.assetProvider =
                assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));

            this.fishTraits = new();
            this.fishEntries = new();
            this.trashEntries = DefaultFishingSource.GetDefaultTrashData();
            this.treasureEntries = DefaultFishingSource.GetDefaultTreasureData();
        }

        public IEnumerable<FishingContent> Reload()
        {
            this.ReloadDefaultFishData();

            yield return new(
                this.manifest,
                this.fishTraits,
                this.fishEntries,
                this.trashEntries,
                this.treasureEntries
            );
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
                this.fishTraits[fishKey] = traits;
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

                        this.fishEntries.AddRange(
                            availabilities.Select(
                                availability =>
                                {
                                    // Clone the 'When' condition
                                    var when = new Dictionary<string, string>();
                                    foreach (var (key, value) in availability.When)
                                    {
                                        when[key] = value;
                                    }

                                    // Add other conditions
                                    if (DefaultFishingSource.FormatSeasons(
                                        seasons,
                                        out var formattedSeasons
                                    ))
                                    {
                                        when["Season"] = formattedSeasons;
                                    }

                                    return new FishEntry(
                                        NamespacedKey.SdvObject(fishId),
                                        new(availability.BaseChance)
                                        {
                                            DepthMultiplier = availability.DepthMultiplier,
                                            MaxDepth = availability.MaxDepth,
                                            WaterTypes = waterTypes,
                                            MinFishingLevel = availability.MinFishingLevel,
                                            IncludeLocations = new() { locationName },
                                            When = when,
                                        }
                                    );
                                }
                            )
                        );
                    }
                }
            }

            // Legendary fish
            var crimsonfishKey = NamespacedKey.SdvObject(159);
            var anglerKey = NamespacedKey.SdvObject(160);
            var legendKey = NamespacedKey.SdvObject(163);
            var mutantCarpKey = NamespacedKey.SdvObject(682);
            var glacierfishKey = NamespacedKey.SdvObject(775);
            var legendaryKeys = new[]
            {
                crimsonfishKey, anglerKey, legendKey, mutantCarpKey, glacierfishKey
            };
            foreach (var legendaryKey in legendaryKeys)
            {
                if (this.fishTraits.TryGetValue(legendaryKey, out var legendaryTraits))
                {
                    legendaryTraits.IsLegendary = true;
                }
                else
                {
                    this.monitor.Log(
                        $"No fish traits found for legendary fish {legendaryKey}. This may cause issues.",
                        LogLevel.Warn
                    );
                }
            }

            // Special entries
            this.fishEntries.AddRange(
                new FishEntry[]
                {
                    // Legendary fish
                    new(
                        crimsonfishKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            WaterTypes = WaterTypes.River,
                            MinFishingLevel = 5,
                            IncludeLocations = new() { "Beach" },
                            When = new()
                            {
                                ["Time"] = "{{Range: 0600, 2000}}",
                                ["Season"] = "Summer",
                            },
                        }
                    ),
                    new(
                        anglerKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            MinFishingLevel = 3,
                            IncludeLocations = new() { "Town" },
                            When = new()
                            {
                                ["Season"] = "Fall",
                            }
                        }
                    ),
                    new(
                        legendKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            WaterTypes = WaterTypes.PondOrOcean,
                            MinFishingLevel = 10,
                            IncludeLocations = new() { "Mountain" },
                            When = new()
                            {
                                ["Time"] = "{{Range: 0600, 2300}}",
                                ["Season"] = "Spring",
                                ["Weather"] = "Rain, Storm",
                            }
                        }
                    ),
                    new(
                        mutantCarpKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            IncludeLocations = new() { "Sewer" },
                        }
                    ),
                    new(
                        glacierfishKey,
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            WaterTypes = WaterTypes.River,
                            MinFishingLevel = 6,
                            IncludeLocations = new() { "Forest" },
                            When = new()
                            {
                                ["Time"] = "{{Range: 0600, 2000}}",
                                ["Season"] = "Winter",
                            }
                        }
                    ),

                    // UndergroundMine
                    new(
                        NamespacedKey.SdvObject(158),
                        new(0.02)
                        {
                            DepthMultiplier = 0.02 / 4,
                            IncludeLocations = new()
                            {
                                "UndergroundMine/0",
                                "UndergroundMine/10"
                            },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(161),
                        new(0.015)
                        {
                            DepthMultiplier = 0.015 / 4,
                            IncludeLocations = new() { "UndergroundMine/40" },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(162),
                        new(0.01)
                        {
                            DepthMultiplier = 0.01 / 4,
                            IncludeLocations = new() { "UndergroundMine/80" },
                        }
                    ),

                    // Submarine
                    new(
                        NamespacedKey.SdvObject(149),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = new() { "Submarine" },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(154),
                        new(0.08)
                        {
                            DepthMultiplier = 0.08 / 4,
                            IncludeLocations = new() { "Submarine" },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(155),
                        new(0.05)
                        {
                            DepthMultiplier = 0.05 / 4,
                            IncludeLocations = new() { "Submarine" },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(798),
                        new(0.28)
                        {
                            DepthMultiplier = 0.28 / 4,
                            IncludeLocations = new() { "Submarine" },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(799),
                        new(0.18)
                        {
                            DepthMultiplier = 0.18 / 4,
                            IncludeLocations = new() { "Submarine" },
                        }
                    ),
                    new(
                        NamespacedKey.SdvObject(800),
                        new(0.1)
                        {
                            DepthMultiplier = 0.1 / 4,
                            IncludeLocations = new() { "Submarine" },
                        }
                    ),
                }
            );

            // Request reload
            this.ReloadRequested?.Invoke(this, EventArgs.Empty);
        }

        private static bool FormatSeasons(
            Seasons seasons,
            [NotNullWhen(true)] out string? formattedSeasons
        )
        {
            switch (seasons)
            {
                case Seasons.None:
                    formattedSeasons = "None";
                    return true;
                case Seasons.All:
                    formattedSeasons = default;
                    return false;
                default:
                    formattedSeasons = string.Join(
                        ", ",
                        DefaultFishingSource.formatSeasons.Where(s => seasons.HasFlag(s))
                    );
                    return true;
            }
        }

        private static bool FormatWeathers(
            Weathers weathers,
            [NotNullWhen(true)] out string? formattedWeathers
        )
        {
            switch (weathers)
            {
                case Weathers.None:
                    formattedWeathers = "None";
                    return true;
                case Weathers.All:
                    formattedWeathers = default;
                    return false;
                default:
                    formattedWeathers = string.Join(
                        ", ",
                        DefaultFishingSource.formatWeathers.Where(w => weathers.HasFlag(w))
                            .SelectMany(
                                w => w switch
                                {
                                    Weathers.Sunny => new[] { "Sun" },
                                    Weathers.Rainy => new[] { "Rain", "Storm" },
                                    Weathers.None => Array.Empty<string>(),
                                    Weathers.All => Array.Empty<string>(),
                                    _ => throw new ArgumentOutOfRangeException(nameof(w), w, null)
                                }
                            )
                    );
                    return true;
            }
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

                var when = new Dictionary<string, string>
                {
                    ["Time"] = $"{{{{ Range: {startTime:0000}, {endTime:0000} }}}}",
                };

                if (DefaultFishingSource.FormatWeathers(weathers, out var formattedWeathers))
                {
                    when["Weather"] = formattedWeathers;
                }

                partialAvailability.Add(
                    new(weightedChance)
                    {
                        DepthMultiplier = depthMultiplier,
                        MaxDepth = maxDepth,
                        MinFishingLevel = minFishingLevel,
                        When = when,
                    }
                );
            }

            // Set traits
            traits = new(dartFrequency, dartBehavior, minSize, maxSize);

            return true;
        }

        private static List<TrashEntry> GetDefaultTrashData()
        {
            return new()
            {
                // Joja Cola
                new(
                    NamespacedKey.SdvObject(167),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Trash
                new(
                    NamespacedKey.SdvObject(168),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Driftwood
                new(
                    NamespacedKey.SdvObject(169),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Broken Glasses
                new(
                    NamespacedKey.SdvObject(170),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Broken CD
                new(
                    NamespacedKey.SdvObject(171),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Soggy Newspaper
                new(
                    NamespacedKey.SdvObject(172),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Seaweed
                new(
                    NamespacedKey.SdvObject(152),
                    new(1.0D) { ExcludeLocations = new() { "Submarine" } }
                ),
                // Green Algae
                new(
                    NamespacedKey.SdvObject(153),
                    new(1.0D)
                    {
                        ExcludeLocations = new()
                        {
                            "Farm",
                            "Submarine"
                        }
                    }
                ),
                // White Algae
                new(
                    NamespacedKey.SdvObject(157),
                    new(1.0D)
                    {
                        IncludeLocations = new()
                        {
                            "BugLand",
                            "Sewers",
                            "WitchSwamp",
                            "UndergroundMines",
                        }
                    }
                ),
                // Pearl
                new(
                    NamespacedKey.SdvObject(797),
                    new(0.01D) { IncludeLocations = new() { "Submarine" } }
                ),
                // Seaweed
                new(
                    NamespacedKey.SdvObject(152),
                    new(0.99D) { IncludeLocations = new() { "Submarine" } }
                ),
            };
        }

        private static List<TreasureEntry> GetDefaultTreasureData()
        {
            // TODO: these needed?
            //new TreasureData(Objects.STRANGE_DOLL1, 0.0025),
            //new TreasureData(Objects.STRANGE_DOLL2, 0.0025),

            return new()
            {
                // Dressed spinner
                new(
                    new(0.025) { MinFishingLevel = 6 },
                    new() { NamespacedKey.SdvObject(687) },
                    allowDuplicates: false
                ),

                // Bait
                new(new(0.25), new() { NamespacedKey.SdvObject(685) }, 2, 4),

                // Archaeology
                new(new(0.025), new() { NamespacedKey.SdvObject(102) }, allowDuplicates: false),
                new(new(0.0625), Enumerable.Range(585, 4).Select(NamespacedKey.SdvObject).ToList()),
                new(new(0.125), Enumerable.Range(96, 32).Select(NamespacedKey.SdvObject).ToList()),

                // Geodes
                new(new(0.2), new() { NamespacedKey.SdvObject(535) }, 1, 3),
                new(new(0.125), new() { NamespacedKey.SdvObject(536) }, 1, 3),
                new(new(0.125), new() { NamespacedKey.SdvObject(537) }, 1, 3),
                new(new(0.0625), new() { NamespacedKey.SdvObject(749) }, 1, 3),

                // Ores + coal
                new(new(0.0075), new() { NamespacedKey.SdvObject(386) }, 1, 3),
                new(new(0.15), new() { NamespacedKey.SdvObject(384) }, 3, 10),
                new(new(0.15), new() { NamespacedKey.SdvObject(380) }, 3, 10),
                new(new(0.15), new() { NamespacedKey.SdvObject(378) }, 3, 10),
                new(new(0.3), new() { NamespacedKey.SdvObject(382) }, 3, 10),

                // Junk
                new(new(0.25), new() { NamespacedKey.SdvObject(388) }, 10, 25),
                new(new(0.25), new() { NamespacedKey.SdvObject(390) }, 10, 25),
                new(new(0.5) { MaxFishingLevel = 1 }, new() { NamespacedKey.SdvObject(770) }, 3, 5),
                new(new(0.005), new() { NamespacedKey.SdvObject(166) }, allowDuplicates: false),
                new(new(0.00025), new() { NamespacedKey.SdvObject(74) }, allowDuplicates: false),

                // Weapons
                new(new(0.001), new() { NamespacedKey.SdvWeapon(14) }, allowDuplicates: false),
                new(new(0.001), new() { NamespacedKey.SdvWeapon(51) }, allowDuplicates: false),

                // Boots
                new(
                    new(0.005),
                    Enumerable.Range(504, 10).Select(NamespacedKey.SdvBoots).ToList(),
                    allowDuplicates: false
                ),

                // Rings
                new(new(0.0025), new() { NamespacedKey.SdvRing(527) }, allowDuplicates: false),
                new(
                    new(0.005),
                    Enumerable.Range(516, 4).Select(NamespacedKey.SdvRing).ToList(),
                    allowDuplicates: false
                ),
                new(
                    new(0.005),
                    Enumerable.Range(529, 6).Select(NamespacedKey.SdvRing).ToList(),
                    allowDuplicates: false
                ),
            };
        }
    }
}
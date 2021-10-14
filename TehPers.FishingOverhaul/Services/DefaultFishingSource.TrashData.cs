using System.Collections.Generic;
using System.Collections.Immutable;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource
    {
        private List<TrashEntry> GetDefaultTrashData()
        {
            return new()
            {
                // Joja cola
                new(
                    NamespacedKey.SdvObject(167),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Trash
                new(
                    NamespacedKey.SdvObject(168),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Driftwood
                new(
                    NamespacedKey.SdvObject(169),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Broken Glasses
                new(
                    NamespacedKey.SdvObject(170),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Broken CD
                new(
                    NamespacedKey.SdvObject(171),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Soggy newspaper
                new(
                    NamespacedKey.SdvObject(172),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Seaweed
                new(
                    NamespacedKey.SdvObject(152),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Green algae
                new(
                    NamespacedKey.SdvObject(153),
                    new(1.0D) { ExcludeLocations = ImmutableArray.Create("Farm", "Submarine") }
                ),
                // White algae
                new(
                    NamespacedKey.SdvObject(157),
                    new(1.0D)
                    {
                        IncludeLocations = ImmutableArray.Create(
                            "BugLand",
                            "Sewers",
                            "WitchSwamp",
                            "UndergroundMines"
                        )
                    }
                ),
                // Pearl
                new(
                    NamespacedKey.SdvObject(797),
                    new(0.01D) { IncludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Seaweed
                new(
                    NamespacedKey.SdvObject(152),
                    new(0.99D) { IncludeLocations = ImmutableArray.Create("Submarine") }
                ),
                // Void mayonnaise
                new(
                    NamespacedKey.SdvObject(308),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("WitchSwamp"),
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag |contains=henchmanGone"] = "false",
                            ["TehPers.FishingOverhaul/HasItem: 308, 1"] = "false",
                            // TODO:  remove this once CP updates
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                ),
                // Secret notes
                new SecretNoteEntry(
                    new(0.08 * 100000)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["HasWalletItem"] = "MagnifyingGlass",
                            ["LocationContext"] = "Valley",
                            ["query: {{Count: {{TehPers.FishingOverhaul/MissingSecretNotes}}}} > 0"] =
                                "true",
                            // TODO: remove this once CP updates
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                ),
                // Journal scraps
                new JournalScrapEntry(
                    new(0.08 * 100000)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["LocationContext"] = "Island",
                            ["query: {{Count: {{TehPers.FishingOverhaul/MissingJournalScraps}}}} > 0"] =
                                "true",
                            // TODO: remove this once CP updates
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary()
                    }
                ),
                // Random golden walnuts
                new GoldenWalnutEntry(
                    new(0.5)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["LocationContext"] = "Island",
                            ["TehPers.FishingOverhaul/RandomGoldenWalnuts"] = "{{Range: 0, 4}}",
                            // TODO: remove once CP fixes mods not being able to use their own tokens
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        CustomEvents = ImmutableArray.Create(
                            new NamespacedKey(this.manifest, "RandomGoldenWalnut")
                        )
                    }
                },
                // Tidepool golden walnut
                new GoldenWalnutEntry(
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandSouthEast"),
                        Position = new()
                        {
                            X = new()
                            {
                                GreaterThanEq = 18,
                                LessThan = 20,
                            },
                            Y = new()
                            {
                                GreaterThanEq = 20,
                                LessThan = 22,
                            },
                        },
                        When = new Dictionary<string, string>
                        {
                            ["TehPers.FishingOverhaul/TidePoolGoldenWalnut"] = "false",
                            // TODO: remove once CP fixes mods not being able to use their own tokens
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        CustomEvents = ImmutableArray.Create(
                            new NamespacedKey(this.manifest, "TidePoolGoldenWalnut")
                        ),
                    },
                },
                // Iridium Krobus
                new(
                    NamespacedKey.SdvFurniture(2396),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Forest"),
                        Position = new()
                        {
                            Y = new()
                            {
                                GreaterThan = 108,
                            },
                        },
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag |contains=caughtIridiumKrobus"] = "false",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        SetFlags = ImmutableArray.Create("caughtIridiumKrobus"),
                    },
                },
                // 'Physics 101'
                new(
                    NamespacedKey.SdvFurniture(2732),
                    new(0.05)
                    {
                        IncludeLocations = ImmutableArray.Create("Caldera"),
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag |contains=CalderaPainting"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) { OnCatch = new() { SetFlags = ImmutableArray.Create("CalderaPainting") } },
                // Pyramid decal
                new(
                    NamespacedKey.SdvFurniture(2334),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("Desert"),
                        Position = new() { Y = new() { GreaterThan = 55 } },
                    }
                ),
                // 'Lifesaver'
                new(
                    NamespacedKey.SdvFurniture(2418),
                    new(0.2) { IncludeLocations = ImmutableArray.Create("Willys Ship") }
                ),
                // Caroline's necklace
                new(
                    NamespacedKey.SdvObject(GameLocation.CAROLINES_NECKLACE_ITEM),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        When = new Dictionary<string, string>
                        {
                            [$"HasFlag |contains={GameLocation.CAROLINES_NECKLACE_MAIL}"] = "false",
                            [$"TehPers.FishingOverhaul/MissingSecretNotes |contains={GameLocation.NECKLACE_SECRET_NOTE_INDEX}"] =
                                "false",
                            // TODO: remove this when CP updates
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new()
                    {
                        StartQuests = ImmutableArray.Create(128, 129),
                        AddMail = ImmutableArray.Create(
                            $"{GameLocation.CAROLINES_NECKLACE_MAIL}%&NL&%"
                        ),
                    },
                },
                // 'Vista'
                new(
                    NamespacedKey.SdvFurniture(2423),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag |contains=gotSpaFishing"] = "false",
                        }.ToImmutableDictionary(),
                    }
                ) { OnCatch = new() { SetFlags = ImmutableArray.Create("gotSpaFishing") } },
                new(
                    NamespacedKey.SdvFurniture(2423),
                    new(0.08)
                    {
                        IncludeLocations = ImmutableArray.Create("Railroad"),
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag"] = "gotSpaFishing",
                        }.ToImmutableDictionary(),
                    }
                ),
                // Decorative trash can
                new(
                    NamespacedKey.SdvFurniture(2427),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        Position = new()
                        {
                            X = new() { LessThan = 30 },
                            Y = new() { LessThan = 30 },
                        }
                    }
                ),
                // Other fountain trash
                new(
                    NamespacedKey.SdvObject(388),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        Position = new()
                        {
                            X = new() { LessThan = 30 },
                            Y = new() { LessThan = 30 },
                        }
                    }
                ),
                new(
                    NamespacedKey.SdvObject(390),
                    new(1)
                    {
                        IncludeLocations = ImmutableArray.Create("Town"),
                        Position = new()
                        {
                            X = new() { LessThan = 30 },
                            Y = new() { LessThan = 30 },
                        }
                    }
                ),
                // Wall basket
                new(
                    NamespacedKey.SdvFurniture(2425),
                    new(0.08) { IncludeLocations = ImmutableArray.Create("Woods") }
                ),
                // Frog hat
                new(
                    NamespacedKey.SdvHat(78),
                    new(0.1) { IncludeLocations = ImmutableArray.Create("IslandFarmCave") }
                ),
                // Foliage print
                new(
                    NamespacedKey.SdvFurniture(2419),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new() { Y = new() { GreaterThan = 72 } },
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag |contains=gotSecretIslandNPainting"] = "false",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new() { SetFlags = ImmutableArray.Create("gotSecretIslandNPainting") }
                },
                new(
                    NamespacedKey.SdvFurniture(2419),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new() { Y = new() { GreaterThan = 72 } },
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag"] = "gotSecretIslandNPainting",
                        }.ToImmutableDictionary(),
                    }
                ),
                // Squirrel figurine
                new(
                    NamespacedKey.SdvFurniture(2814),
                    new(0.25)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new()
                        {
                            X = new() { LessThan = 4 },
                            Y = new() { LessThan = 35 },
                        },
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag |contains=gotSecretIslandNSquirrel"] = "false",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    OnCatch = new() { SetFlags = ImmutableArray.Create("gotSecretIslandNSquirrel") }
                },
                new(
                    NamespacedKey.SdvFurniture(2814),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandNorth"),
                        Position = new()
                        {
                            X = new() { LessThan = 4 },
                            Y = new() { LessThan = 35 },
                        },
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag"] = "gotSecretIslandNSquirrel",
                        }.ToImmutableDictionary(),
                    }
                ),
                // Gourmand's statue
                new(
                    NamespacedKey.SdvFurniture(2332),
                    new(0.05) { IncludeLocations = ImmutableArray.Create("IslandSouthEastCave") }
                ),
                // Snake skull
                new(
                    NamespacedKey.SdvObject(857),
                    new(0.1)
                    {
                        IncludeLocations = ImmutableArray.Create("IslandWest"),
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag"] = "islandNorthCaveOpened",
                        }.ToImmutableDictionary(),
                    }
                )
            };
        }
    }
}
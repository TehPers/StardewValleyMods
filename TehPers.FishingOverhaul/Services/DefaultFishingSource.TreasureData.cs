using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TehPers.Core.Api.Gameplay;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal partial class DefaultFishingSource
    {
        private List<TreasureEntry> GetDefaultTreasureData()
        {
            // TODO: these needed?
            //new TreasureData(Objects.STRANGE_DOLL1, 0.0025),
            //new TreasureData(Objects.STRANGE_DOLL2, 0.0025),

            const double archaeologyChance = 0.015625;

            return new()
            {
                // Dressed spinner
                new(
                    new(0.025) { MinFishingLevel = 6 },
                    ImmutableArray.Create(NamespacedKey.SdvObject(687))
                ) { AllowDuplicates = false },

                // Bait
                new(new(0.25), ImmutableArray.Create(NamespacedKey.SdvObject(685)))
                {
                    MinQuantity = 2,
                    MaxQuantity = 4,
                },

                // Archaeology
                new LostBookEntry(
                    new(0.025 * 100000)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["TehPers.FishingOverhaul/BooksFound"] = "{{Range: 0, 19}}",
                            // TODO: remove this when CP updates
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary(),
                    }
                )
                {
                    AllowDuplicates = false,
                    OnCatch = new()
                    {
                        CustomEvents = ImmutableArray.Create(
                            new NamespacedKey(this.manifest, "LostBook")
                        ),
                    },
                },
                new(
                    new(archaeologyChance * 4),
                    Enumerable.Range(585, 4).Select(NamespacedKey.SdvObject).ToImmutableArray()
                ),
                new(
                    new(archaeologyChance * 6),
                    Enumerable.Range(96, 6).Select(NamespacedKey.SdvObject).ToImmutableArray()
                ),
                new(
                    new(archaeologyChance * 25),
                    Enumerable.Range(103, 25).Select(NamespacedKey.SdvObject).ToImmutableArray()
                ),

                // Geodes
                new(new(0.2), ImmutableArray.Create(NamespacedKey.SdvObject(535)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                },
                new(new(0.125), ImmutableArray.Create(NamespacedKey.SdvObject(536)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                },
                new(new(0.125), ImmutableArray.Create(NamespacedKey.SdvObject(537)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                },
                new(new(0.0625), ImmutableArray.Create(NamespacedKey.SdvObject(749)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                },

                // Ores + coal
                new(new(0.0075), ImmutableArray.Create(NamespacedKey.SdvObject(386)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 3,
                },
                new(new(0.15), ImmutableArray.Create(NamespacedKey.SdvObject(384)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10,
                },
                new(new(0.15), ImmutableArray.Create(NamespacedKey.SdvObject(380)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10
                },
                new(new(0.15), ImmutableArray.Create(NamespacedKey.SdvObject(378)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10,
                },
                new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(382)))
                {
                    MinQuantity = 3,
                    MaxQuantity = 10,
                },

                // Gemstones
                new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(60)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(62)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(64)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(66)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(68)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.5), ImmutableArray.Create(NamespacedKey.SdvObject(70)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.028), ImmutableArray.Create(NamespacedKey.SdvObject(72)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 2,
                },
                new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(82)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(84)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },
                new(new(0.3), ImmutableArray.Create(NamespacedKey.SdvObject(86)))
                {
                    MinQuantity = 1,
                    MaxQuantity = 6,
                },

                // Junk
                new(new(0.25), ImmutableArray.Create(NamespacedKey.SdvObject(388)))
                {
                    MinQuantity = 10,
                    MaxQuantity = 25,
                },
                new(new(0.25), ImmutableArray.Create(NamespacedKey.SdvObject(390)))
                {
                    MinQuantity = 10,
                    MaxQuantity = 25,
                },
                new(
                    new(0.5) { MaxFishingLevel = 1 },
                    ImmutableArray.Create(NamespacedKey.SdvObject(770))
                )
                {
                    MinQuantity = 3,
                    MaxQuantity = 5,
                },
                new(new(0.005), ImmutableArray.Create(NamespacedKey.SdvObject(166)))
                {
                    AllowDuplicates = false,
                },
                new(new(0.00025), ImmutableArray.Create(NamespacedKey.SdvObject(74)))
                {
                    AllowDuplicates = false,
                },
                new(
                    new(0.01)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["HasFlag: hostPlayer"] = "Farm_Eternal",
                        }.ToImmutableDictionary(),
                    },
                    ImmutableArray.Create(NamespacedKey.SdvObject(928))
                ),

                // Strange doll
                new(
                    new(0.01),
                    Enumerable.Range(126, 2).Select(NamespacedKey.SdvObject).ToImmutableArray()
                ),

                // Rice shoot
                new(
                    new(0.1) { Seasons = Seasons.Spring },
                    ImmutableArray.Create(NamespacedKey.SdvObject(273))
                )
                {
                    MinQuantity = 2,
                    MaxQuantity = 11,
                },

                // Qi beans
                new(
                    new(0.33)
                    {
                        When = new Dictionary<string, string>
                        {
                            ["TehPers.FishingOverhaul/SpecialOrderRuleActive"] =
                                "DROP_QI_BEANS",
                            ["HasMod"] = "TehPers.FishingOverhaul",
                        }.ToImmutableDictionary()
                    },
                    ImmutableArray.Create(NamespacedKey.SdvObject(890))
                )
                {
                    MinQuantity = 1,
                    MaxQuantity = 4,
                },

                // Weapons
                new(new(0.001), ImmutableArray.Create(NamespacedKey.SdvWeapon(14)))
                {
                    AllowDuplicates = false,
                },
                new(new(0.001), ImmutableArray.Create(NamespacedKey.SdvWeapon(51)))
                {
                    AllowDuplicates = false,
                },

                // Boots
                new(
                    new(0.005),
                    Enumerable.Range(504, 10).Select(NamespacedKey.SdvBoots).ToImmutableArray()
                ) { AllowDuplicates = false },

                // Rings
                new(new(0.0025), ImmutableArray.Create(NamespacedKey.SdvRing(527)))
                {
                    AllowDuplicates = false,
                },
                new(
                    new(0.005),
                    Enumerable.Range(516, 4).Select(NamespacedKey.SdvRing).ToImmutableArray()
                ) { AllowDuplicates = false },
                new(
                    new(0.005),
                    Enumerable.Range(529, 6).Select(NamespacedKey.SdvRing).ToImmutableArray()
                ) { AllowDuplicates = false },
            };
        }
    }
}
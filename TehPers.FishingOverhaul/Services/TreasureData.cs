using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    public class TreasureData
    {
        public List<TreasureEntry> Entries { get; }

        public TreasureData(List<TreasureEntry>? availabilities)
        {
            this.Entries = availabilities ?? new();
        }

        public static TreasureData GetDefaultTreasureData()
        {
            // TODO: these needed?
            //new TreasureData(Objects.STRANGE_DOLL1, 0.0025),
            //new TreasureData(Objects.STRANGE_DOLL2, 0.0025),

            var availabilities = new List<TreasureEntry>
            {
                // Dressed spinner
                new(new(0.025, minFishingLevel: 6), new() { NamespacedKey.SdvObject(687) }, allowDuplicates: false),

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
                new(new(0.5, maxFishingLevel: 1), new() { NamespacedKey.SdvObject(770) }, 3, 5),

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

            return new(availabilities);
        }
    }
}
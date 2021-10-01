using System.Collections.Generic;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Loading
{
    public class TrashData
    {
        public List<TrashAvailability> Availabilities { get; }

        public TrashData(List<TrashAvailability>? availabilities)
        {
            this.Availabilities = availabilities ?? new();
        }

        public static TrashData GetDefaultTrashData()
        {
            var availabilities = new List<TrashAvailability>
            {
                // Joja Cola
                new(NamespacedKey.SdvObject(167), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Trash
                new(NamespacedKey.SdvObject(168), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Driftwood
                new(NamespacedKey.SdvObject(169), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Broken Glasses
                new(NamespacedKey.SdvObject(170), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Broken CD
                new(NamespacedKey.SdvObject(171), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Soggy Newspaper
                new(NamespacedKey.SdvObject(172), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Seaweed
                new(NamespacedKey.SdvObject(152), 1.0D, excludeLocations: new List<string> { "Submarine" }),
                // Green Algae
                new(
                    NamespacedKey.SdvObject(153),
                    1.0D,
                    excludeLocations: new List<string> { "Farm", "Submarine" }
                ),
                // White Algae
                new(
                    NamespacedKey.SdvObject(157),
                    1.0D,
                    includeLocations: new List<string>
                    {
                        "BugLand",
                        "Sewers",
                        "WitchSwamp",
                        "UndergroundMines",
                    }
                ),
                // Pearl
                new(
                    NamespacedKey.SdvObject(797),
                    0.01D,
                    includeLocations: new List<string> { "Submarine" }
                ),
                // Seaweed
                new(
                    NamespacedKey.SdvObject(152),
                    0.99D,
                    includeLocations: new List<string> { "Submarine" }
                ),
            };

            return new TrashData(availabilities);
        }
    }
}
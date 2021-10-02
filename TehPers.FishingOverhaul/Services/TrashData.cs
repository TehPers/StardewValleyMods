using System.Collections.Generic;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    public class TrashData
    {
        public List<TrashEntry> Entries { get; }

        public TrashData(List<TrashEntry>? availabilities)
        {
            this.Entries = availabilities ?? new();
        }

        public static TrashData GetDefaultTrashData()
        {
            var availabilities = new List<TrashEntry>
            {
                // Joja Cola
                new(NamespacedKey.SdvObject(167), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Trash
                new(NamespacedKey.SdvObject(168), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Driftwood
                new(NamespacedKey.SdvObject(169), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Broken Glasses
                new(NamespacedKey.SdvObject(170), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Broken CD
                new(NamespacedKey.SdvObject(171), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Soggy Newspaper
                new(NamespacedKey.SdvObject(172), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Seaweed
                new(NamespacedKey.SdvObject(152), new(1.0D, excludeLocations: new() { "Submarine" })),
                // Green Algae
                new(NamespacedKey.SdvObject(153), new(1.0D, excludeLocations: new() { "Farm", "Submarine" })),
                // White Algae
                new(
                    NamespacedKey.SdvObject(157),
                    new(1.0D, includeLocations: new() { "BugLand", "Sewers", "WitchSwamp", "UndergroundMines", })
                ),
                // Pearl
                new(NamespacedKey.SdvObject(797), new(0.01D, includeLocations: new() { "Submarine" })),
                // Seaweed
                new(NamespacedKey.SdvObject(152), new(0.99D, includeLocations: new() { "Submarine" })),
            };

            return new(availabilities);
        }
    }
}
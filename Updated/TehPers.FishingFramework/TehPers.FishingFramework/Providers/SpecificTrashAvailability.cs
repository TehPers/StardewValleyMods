using System;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.Chrono;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Providers
{
    internal class SpecificTrashAvailability : ITrashAvailability
    {
        public NamespacedId ItemId { get; }
        public string LocationName { get; }
        public double Weight { get; }
        public bool InvertLocations { get; }

        public SpecificTrashAvailability(NamespacedId itemId, string locationName, double weight = 1d, bool invertLocations = false)
        {
            this.ItemId = itemId;
            this.LocationName = locationName;
            this.Weight = weight;
            this.InvertLocations = invertLocations;
        }

        public double GetWeightedChance(Farmer who, GameLocation location, Weathers weather, WaterTypes water, SDateTime dateTime, int? mineLevel = null)
        {
            if (location.Name.Equals(this.LocationName, StringComparison.Ordinal) ^ this.InvertLocations)
            {
                return this.Weight;
            }

            return 0;
        }
    }
}
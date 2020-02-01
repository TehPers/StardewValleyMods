using System;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.Chrono;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Providers
{
    internal class GlobalTrashAvailability : ITrashAvailability
    {
        public NamespacedId ItemId { get; }

        public GlobalTrashAvailability(NamespacedId itemId)
        {
            this.ItemId = itemId;
        }

        public double GetWeightedChance(Farmer who, GameLocation location, Weathers weather, WaterTypes water, SDateTime dateTime, int? mineLevel = null)
        {
            return location.Name.Equals("Submarine", StringComparison.Ordinal) ? 0 : 1;
        }
    }
}
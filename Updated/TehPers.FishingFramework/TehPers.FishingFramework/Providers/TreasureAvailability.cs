using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.Chrono;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Providers
{
    [JsonDescribe]
    internal class TreasureAvailability : ITreasureAvailability
    {
        public IEnumerable<NamespacedId> ItemIds { get; }
        public double Weight { get; }
        public int MinQuantity { get; }
        public int MaxQuantity { get; }
        public int MinLevel { get; }
        public int MaxLevel { get; }
        public bool AllowDuplicates { get; }

        public TreasureAvailability(IEnumerable<NamespacedId> ids, double weight, int minQuantity = 1, int maxQuantity = 1, int minLevel = 0, int maxLevel = int.MaxValue, bool allowDuplicates = true)
        {
            this.ItemIds = ids.ToArray();
            this.Weight = weight;
            this.MinQuantity = minQuantity;
            this.MaxQuantity = maxQuantity;
            this.MinLevel = minLevel;
            this.MaxLevel = maxLevel;
            this.AllowDuplicates = allowDuplicates;
        }

        public double GetWeightedChance(Farmer who, GameLocation location, Weathers weather, WaterTypes water, SDateTime dateTime, int? mineLevel = null)
        {
            if (!this.MeetsCriteria(who))
            {
                return 0;
            }

            return this.Weight;
        }

        private bool MeetsCriteria(Farmer who)
        {
            return who.FishingLevel >= this.MinLevel && who.FishingLevel <= this.MaxLevel;
        }
    }
}
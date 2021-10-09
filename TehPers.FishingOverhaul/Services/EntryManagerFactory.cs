using System;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    internal class EntryManagerFactory<TEntry, TAvailability>
        where TEntry : Entry<TAvailability>
        where TAvailability : AvailabilityInfo
    {
        private readonly ChanceCalculatorFactory<TAvailability> chanceCalculatorFactory;

        public EntryManagerFactory(ChanceCalculatorFactory<TAvailability> chanceCalculatorFactory)
        {
            this.chanceCalculatorFactory = chanceCalculatorFactory
                ?? throw new ArgumentNullException(nameof(chanceCalculatorFactory));
        }

        public EntryManager<TEntry, TAvailability> Create(IManifest owner, TEntry entry)
        {
            return new(this.chanceCalculatorFactory.Create(owner, entry.AvailabilityInfo), entry);
        }
    }
}
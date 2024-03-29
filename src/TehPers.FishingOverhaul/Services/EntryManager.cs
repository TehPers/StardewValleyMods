﻿using System;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class EntryManager<TEntry, TAvailability>
        where TEntry : Entry<TAvailability>
        where TAvailability : AvailabilityInfo
    {
        public TEntry Entry { get; }
        public ChanceCalculator ChanceCalculator { get; }

        public EntryManager(ChanceCalculator chanceCalculator, TEntry entry)
        {
            this.ChanceCalculator = chanceCalculator
                ?? throw new ArgumentNullException(nameof(chanceCalculator));
            this.Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }
    }
}

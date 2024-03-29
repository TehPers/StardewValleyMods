﻿using System;
using ContentPatcher;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class CalculatorFactory
    {
        private readonly Lazy<IContentPatcherAPI> contentPatcherApiFactory;
        private readonly IManifest fishingManifest;
        private readonly IMonitor monitor;

        public CalculatorFactory(
            Lazy<IContentPatcherAPI> contentPatcherApiFactory,
            IManifest fishingManifest,
            IMonitor monitor
        )
        {
            this.contentPatcherApiFactory = contentPatcherApiFactory
                ?? throw new ArgumentNullException(nameof(contentPatcherApiFactory));
            this.fishingManifest = fishingManifest
                ?? throw new ArgumentNullException(nameof(fishingManifest));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public ConditionsCalculator Conditions(IManifest owner, AvailabilityConditions conditions)
        {
            return new(
                this.monitor,
                this.contentPatcherApiFactory.Value,
                this.fishingManifest,
                owner,
                conditions
            );
        }

        public ChanceCalculator Chances(IManifest owner, AvailabilityInfo info)
        {
            return new(
                this.monitor,
                this.contentPatcherApiFactory.Value,
                this.fishingManifest,
                owner,
                info
            );
        }
    }
}

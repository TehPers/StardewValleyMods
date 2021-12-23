using System;
using ContentPatcher;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class ChanceCalculatorFactory<T>
        where T : AvailabilityInfo
    {
        private readonly Lazy<IContentPatcherAPI> contentPatcherApiFactory;
        private readonly IManifest fishingManifest;
        private readonly IMonitor monitor;

        public ChanceCalculatorFactory(
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

        public ChanceCalculator<T> Create(IManifest owner, T availabilityInfo)
        {
            return new(
                this.monitor,
                this.contentPatcherApiFactory.Value,
                this.fishingManifest,
                owner,
                availabilityInfo
            );
        }
    }
}
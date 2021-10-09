using System;
using ContentPatcher;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    internal class ChanceCalculatorFactory<T>
        where T : AvailabilityInfo
    {
        private readonly Func<IContentPatcherAPI> contentPatcherApiFactory;
        private readonly IManifest fishingManifest;

        public ChanceCalculatorFactory(
            Func<IContentPatcherAPI> contentPatcherApiFactory,
            IManifest fishingManifest
        )
        {
            this.contentPatcherApiFactory = contentPatcherApiFactory
                ?? throw new ArgumentNullException(nameof(contentPatcherApiFactory));
            this.fishingManifest = fishingManifest
                ?? throw new ArgumentNullException(nameof(fishingManifest));
        }

        public ChanceCalculator<T> Create(IManifest owner, T availabilityInfo)
        {
            return new(
                this.contentPatcherApiFactory(),
                this.fishingManifest,
                owner,
                availabilityInfo
            );
        }
    }
}
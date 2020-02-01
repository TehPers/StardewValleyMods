using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api;
using TehPers.Core.Api.Configuration;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Providers;
using TehPers.FishingFramework.Config;

namespace TehPers.FishingFramework.Providers
{
    internal sealed class BaseFishProvider : IFishProvider, IDisposable
    {
        private readonly IConfiguration<FishEntriesConfiguration> fishEntries;
        private readonly IConfiguration<FishTraitsConfiguration> fishTraits;

        private IFishAvailability[] fish;

        public IEnumerable<IFishAvailability> Fish => this.fish;

        public BaseFishProvider(IConfiguration<FishEntriesConfiguration> fishEntries, IConfiguration<FishTraitsConfiguration> fishTraits)
        {
            this.fishEntries = fishEntries;
            this.fishTraits = fishTraits;

            fishEntries.Changed += this.FishEntriesOnChanged;
            fishTraits.Changed += this.FishTraitsOnChanged;

            this.UpdateFish(fishEntries.Value, fishTraits.Value);
        }

        private void FishEntriesOnChanged(object sender, ConfigurationChangedEventArgs<FishEntriesConfiguration> e)
        {
            this.UpdateFish(this.fishEntries.Value, this.fishTraits.Value);
        }

        private void FishTraitsOnChanged(object sender, ConfigurationChangedEventArgs<FishTraitsConfiguration> e)
        {
            this.UpdateFish(this.fishEntries.Value, this.fishTraits.Value);
        }

        private void UpdateFish(FishEntriesConfiguration fishConfig, FishTraitsConfiguration traitsConfig)
        {
            this.fish = fishConfig.PossibleFish.Cast<IFishAvailability>().ToArray();
        }

        public bool TryGetTraits(NamespacedId fishId, out IFishTraits traits)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.fishEntries.Changed -= this.FishEntriesOnChanged;
            this.fishTraits.Changed -= this.FishTraitsOnChanged;
        }
    }
}
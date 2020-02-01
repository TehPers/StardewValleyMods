using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DependencyInjection;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Providers;

namespace TehPers.FishingFramework.Providers
{
    internal class DefaultFishProvider : IDefaultFishProvider
    {
        private readonly IAssetProvider gameAssets;
        private readonly Lazy<IFishAvailability[]> fish;

        public IEnumerable<IFishAvailability> Fish => this.fish.Value;

        public DefaultFishProvider(
            [ContentSource(ContentSource.GameContent)]
            IAssetProvider gameAssets)
        {
            this.gameAssets = gameAssets ?? throw new ArgumentNullException(nameof(gameAssets));

            this.fish = new Lazy<IFishAvailability[]>(() => this.GenerateFishAvailabilities().ToArray());
        }

        private IEnumerable<IFishAvailability> GenerateFishAvailabilities()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTraits(NamespacedId fishId, out IFishTraits traits)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DependencyInjection;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Providers;

namespace TehPers.FishingFramework.Providers
{
    internal class DefaultFishingProvider : IDefaultFishProvider, IDefaultTreasureProvider, IDefaultTrashProvider
    {
        private readonly IAssetProvider gameAssets;
        private readonly Lazy<List<IFishAvailability>> fish;
        private readonly Lazy<List<ITreasureAvailability>> treasure;
        private readonly Lazy<List<ITrashAvailability>> trash;

        public IEnumerable<IFishAvailability> Fish => this.fish.Value;
        public IEnumerable<ITreasureAvailability> Treasure => this.treasure.Value;
        public IEnumerable<ITrashAvailability> Trash => this.trash.Value;

        public DefaultFishingProvider([ContentSource(ContentSource.GameContent)] IAssetProvider gameAssets)
        {
            this.gameAssets = gameAssets ?? throw new ArgumentNullException(nameof(gameAssets));

            this.fish = new Lazy<List<IFishAvailability>>(this.GenerateFishAvailabilities);
            this.treasure = new Lazy<List<ITreasureAvailability>>(this.GenerateTreasureAvailabilities);
            this.trash = new Lazy<List<ITrashAvailability>>(this.GenerateTrashAvailabilities);
        }

        private List<IFishAvailability> GenerateFishAvailabilities()
        {
            throw new NotImplementedException();
        }

        private List<ITreasureAvailability> GenerateTreasureAvailabilities()
        {
            throw new NotImplementedException();
        }

        private List<ITrashAvailability> GenerateTrashAvailabilities()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTraits(NamespacedId fishId, out IFishTraits traits)
        {
            throw new NotImplementedException();
        }
    }
}
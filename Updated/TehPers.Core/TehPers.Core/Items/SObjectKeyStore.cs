using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.Items
{
    public class SObjectKeyStore : BaseKeyStore
    {
        private readonly IDataStore<Dictionary<NamespacedId, int>> indexStore;
        private readonly IAssetProvider assetProvider;

        public SObjectKeyStore(
            IDataStore<Dictionary<NamespacedId, int>> indexStore,
            [ContentSource(ContentSource.GameContent)]
            IAssetProvider assetProvider)
            : base(indexStore)
        {
            this.indexStore = indexStore;
            this.assetProvider = assetProvider;
        }

        protected override Dictionary<NamespacedId, int> ConstructIdDictionary()
        {
            var rawData = this.assetProvider.Load<Dictionary<int, string>>("Data/ObjectInformation");
            return rawData.Keys.ToDictionary(NamespacedId.FromObjectIndex);
        }
    }
}
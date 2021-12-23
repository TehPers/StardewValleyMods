using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource : IFishingContentSource
    {
        private readonly IManifest manifest;
        private readonly IAssetProvider assetProvider;

        private readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        private readonly List<FishEntry> fishEntries;
        private readonly List<TrashEntry> trashEntries;
        private readonly List<TreasureEntry> treasureEntries;

        public event EventHandler? ReloadRequested;

        public DefaultFishingSource(
            IManifest manifest,
            [ContentSource(ContentSource.GameContent)] IAssetProvider assetProvider
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.assetProvider =
                assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));

            this.fishTraits = new();
            this.fishEntries = new();
            this.trashEntries = this.GetDefaultTrashData();
            this.treasureEntries = this.GetDefaultTreasureData();
        }

        public IEnumerable<FishingContent> Reload()
        {
            this.ReloadDefaultFishData();

            yield return new(this.manifest)
            {
                SetFishTraits = this.fishTraits.ToImmutableDictionary(),
                AddFish = this.fishEntries.ToImmutableArray(),
                AddTrash = this.trashEntries.ToImmutableArray(),
                AddTreasure = this.treasureEntries.ToImmutableArray(),
            };
        }
    }
}
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource : IFishingContentSource
    {
        private const double prioritizedTier = 10d;
        private const double specialItemTier = 20d;
        private const double questItemTier = 30d;

        private readonly IManifest manifest;
        private readonly IAssetProvider assetProvider;

        private readonly List<FishingContent> defaultContent = new();

        public event EventHandler? ReloadRequested;

        public DefaultFishingSource(
            IManifest manifest,
            [ContentSource(ContentSource.GameContent)] IAssetProvider assetProvider
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.assetProvider =
                assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        }

        public IEnumerable<FishingContent> Reload(IMonitor monitor)
        {
            // Reload default content
            this.defaultContent.Clear();
            this.defaultContent.Add(this.GetDefaultFishData(monitor));
            this.defaultContent.Add(this.GetDefaultTrashData());
            this.defaultContent.Add(this.GetDefaultTreasureData());
            this.defaultContent.Add(this.GetDefaultEffectData());

            return this.defaultContent;
        }
    }
}

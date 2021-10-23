using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Config.ContentPacks;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed class ContentPackSource : IFishingContentSource
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IJsonProvider jsonProvider;

        public ContentPackSource(IModHelper helper, IMonitor monitor, IJsonProvider jsonProvider)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.jsonProvider =
                jsonProvider ?? throw new ArgumentNullException(nameof(jsonProvider));
        }

        public IEnumerable<FishingContent> Reload()
        {
            // Load content packs
            foreach (var pack in this.helper.ContentPacks.GetOwned())
            {
                // Content
                if (!this.TryRead<FishingContentPack>(pack, "content.json", out var contentPack))
                {
                    continue;
                }

                // Fish traits
                // TODO: Remove this when compatibility with the old content pack system is no longer needed
                if (!this.TryRead<FishTraitsPack>(pack, "fishTraits.json", out var fishTraits))
                {
                    continue;
                }

                // Fish
                // TODO: Remove this when compatibility with the old content pack system is no longer needed
                if (!this.TryRead<FishPack>(pack, "fish.json", out var fish))
                {
                    continue;
                }

                // Trash
                // TODO: Remove this when compatibility with the old content pack system is no longer needed
                if (!this.TryRead<TrashPack>(pack, "trash.json", out var trash))
                {
                    continue;
                }

                // Treasure
                // TODO: Remove this when compatibility with the old content pack system is no longer needed
                if (!this.TryRead<TreasurePack>(pack, "treasure.json", out var treasure))
                {
                    continue;
                }

                // Load content
                var content = new FishingContent(pack.Manifest);
                content = contentPack?.AddTo(content, string.Empty, pack, this.jsonProvider, this.monitor) ?? content;
                content = fishTraits?.AddTo(content) ?? content;
                content = fish?.AddTo(content) ?? content;
                content = trash?.AddTo(content) ?? content;
                content = treasure?.AddTo(content) ?? content;
                yield return content;
            }
        }

        private bool TryRead<T>(IContentPack pack, string path, [NotNullWhen(true)] out T? result)
            where T : class
        {
            try
            {
                // Try to read the file
                result = this.jsonProvider.ReadJson<T>(
                    path,
                    new ContentPackAssetProvider(pack),
                    null
                );
                return result is not null;
            }
            catch (Exception ex)
            {
                this.monitor.Log(
                    $"Error loading content pack '{pack.Manifest.UniqueID}'. The file {path} is invalid.\n{ex}",
                    LogLevel.Error
                );
                result = default;
                return false;
            }
        }
    }
}
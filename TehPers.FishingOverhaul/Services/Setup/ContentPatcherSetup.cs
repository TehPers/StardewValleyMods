using System;
using System.Collections.Generic;
using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal sealed class ContentPatcherSetup : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IManifest manifest;
        private readonly Lazy<IContentPatcherAPI> contentPatcherApi;

        public ContentPatcherSetup(
            IModHelper helper,
            IManifest manifest,
            Lazy<IContentPatcherAPI> contentPatcherApi
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.contentPatcherApi = contentPatcherApi
                ?? throw new ArgumentNullException(nameof(contentPatcherApi));
        }

        public void Setup()
        {
            this.helper.Events.GameLoop.GameLaunched += this.SetupTokens;
        }

        public void Dispose()
        {
            this.helper.Events.GameLoop.GameLaunched -= this.SetupTokens;
        }

        private void SetupTokens(object? sender, GameLaunchedEventArgs e)
        {
            var cpApi = this.contentPatcherApi.Value;
            cpApi.RegisterToken(
                this.manifest,
                "RandomGoldenWalnuts",
                ContentPatcherSetup.GetRandomGoldenWalnuts
            );
            cpApi.RegisterToken(
                this.manifest,
                "TidePoolGoldenWalnut",
                ContentPatcherSetup.GetTidePoolGoldenWalnut
            );
        }

        private static IEnumerable<string>? GetRandomGoldenWalnuts()
        {
            if (Game1.player is not { team: { limitedNutDrops: { } limitedNutDrops } })
            {
                return null;
            }

            return limitedNutDrops.TryGetValue("IslandFishing", out var fishingNuts)
                ? new[] { fishingNuts.ToString("G") }
                : new[] { "0" };
        }

        private static IEnumerable<string>? GetTidePoolGoldenWalnut()
        {
            if (Game1.player is not { team: { } team })
            {
                return null;
            }

            return team.collectedNutTracker.TryGetValue("StardropPool", out var gotNut) && gotNut
                ? new[] { "true" }
                : new[] { "false" };
        }
    }
}
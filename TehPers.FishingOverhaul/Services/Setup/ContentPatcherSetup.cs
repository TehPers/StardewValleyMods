using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal sealed class ContentPatcherSetup : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IManifest manifest;
        private readonly Lazy<IContentPatcherAPI> contentPatcherApi;
        private readonly IAssetProvider gameAssets;

        public ContentPatcherSetup(
            IModHelper helper,
            IManifest manifest,
            Lazy<IContentPatcherAPI> contentPatcherApi,
            [ContentSource(ContentSource.GameContent)] IAssetProvider gameAssets
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.contentPatcherApi = contentPatcherApi
                ?? throw new ArgumentNullException(nameof(contentPatcherApi));
            this.gameAssets = gameAssets ?? throw new ArgumentNullException(nameof(gameAssets));
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
            cpApi.RegisterToken(this.manifest, "BooksFound", new BooksFoundToken());
            cpApi.RegisterToken(this.manifest, "HasItem", new HasItemToken());
            cpApi.RegisterToken(this.manifest, "MissingSecretNotes", this.GetMissingSecretNotes);
            cpApi.RegisterToken(
                this.manifest,
                "MissingJournalScraps",
                this.GetMissingJournalScraps
            );
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

        private IEnumerable<string>? GetMissingSecretNotes()
        {
            return Game1.player is { secretNotesSeen: { } secretNotesSeen } player
                ? this.gameAssets.Load<Dictionary<int, string>>(@"Data\SecretNotes")
                    .Keys.Where(id => id < GameLocation.JOURNAL_INDEX)
                    .Except(secretNotesSeen)
                    .Where(
                        id => !player.hasItemInInventoryNamed(
                                $"Secret Note #{id - GameLocation.JOURNAL_INDEX}"
                            )
                            && (id != 10 || player.mailReceived.Contains("QiChallengeComplete"))
                    )
                    .Select(id => id.ToString("G"))
                : null;
        }

        private IEnumerable<string>? GetMissingJournalScraps()
        {
            return Game1.player is { secretNotesSeen: { } secretNotesSeen } player
                ? this.gameAssets.Load<Dictionary<int, string>>(@"Data\SecretNotes")
                    .Keys.Where(id => id >= GameLocation.JOURNAL_INDEX)
                    .Except(secretNotesSeen)
                    .Where(
                        id => !player.hasItemInInventoryNamed(
                            $"Journal Scrap #{id - GameLocation.JOURNAL_INDEX}"
                        )
                    )
                    .Select(id => (id - GameLocation.JOURNAL_INDEX).ToString("G"))
                : null;
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

        [SuppressMessage(
            "Style",
            "IDE0060:Remove unused parameter",
            Justification = "Used by Content Patcher."
        )]
        [SuppressMessage(
            "Performance",
            "CA1822:Mark members as static",
            Justification = "Used by Content Patcher."
        )]
        [SuppressMessage(
            "ReSharper",
            "UnusedMember.Local",
            Justification = "Used by Content Patcher."
        )]
        private class BooksFoundToken
        {
            public bool AllowsInput() => true;
            public bool CanHaveMultipleValues(string? input = null) => false;
            public bool IsReady() => Game1.player is not null;

            public IEnumerable<string> GetValues(string? input)
            {
                // Get archaeology ID
                if (!int.TryParse(input, out var id))
                {
                    return Enumerable.Empty<string>();
                }

                // Get player's archaeology
                if (Game1.player is not { archaeologyFound: { } archaeologyFound })
                {
                    return Enumerable.Empty<string>();
                }

                // Get the stats for this ID
                if (!archaeologyFound.TryGetValue(id, out var value))
                {
                    return Enumerable.Empty<string>();
                }

                return new[] { value[0].ToString("G") };
            }
        }

        [SuppressMessage(
            "Style",
            "IDE0060:Remove unused parameter",
            Justification = "Used by Content Patcher."
        )]
        [SuppressMessage(
            "Performance",
            "CA1822:Mark members as static",
            Justification = "Used by Content Patcher."
        )]
        [SuppressMessage(
            "ReSharper",
            "UnusedMember.Local",
            Justification = "Used by Content Patcher."
        )]
        private class HasItemToken
        {
            public bool AllowsInput() => true;
            public bool CanHaveMultipleValues(string? input = null) => true;
            public bool IsReady() => Game1.player is not null;

            public IEnumerable<string> GetValues(string? input)
            {
                // Ensure input is not null
                if (input is null)
                {
                    return Enumerable.Empty<string>();
                }

                // Split input
                var args = input.Split(',', StringSplitOptions.TrimEntries);
                if (args.Length != 2)
                {
                    return Enumerable.Empty<string>();
                }

                // Get item ID
                if (!int.TryParse(args[0], out var index))
                {
                    return Enumerable.Empty<string>();
                }

                // Get quantity
                if (!int.TryParse(args[1], out var quantity))
                {
                    return Enumerable.Empty<string>();
                }

                // Get player's archaeology
                if (Game1.player is not { } player)
                {
                    return Enumerable.Empty<string>();
                }

                return new[] { player.hasItemInInventory(index, quantity) ? "true" : "false" };
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;

namespace TehPers.FishingOverhaul.Services.Tokens
{
    internal class MissingJournalScrapsToken : MissingNotesToken
    {
        public MissingJournalScrapsToken(
            IAssetTracker assetTracker,
            [ContentSource(ContentSource.GameContent)] IAssetProvider gameAssets
        )
            : base(assetTracker, gameAssets)
        {
        }

        public override IEnumerable<string> GetValues(string? input)
        {
            if (Game1.player is not { secretNotesSeen: { } secretNotesSeen } player)
            {
                return Enumerable.Empty<string>();
            }

            return this.SecretNotes.Keys.Where(id => id >= GameLocation.JOURNAL_INDEX)
                .Except(secretNotesSeen)
                .Where(
                    id => !player.hasItemInInventoryNamed(
                            $"Journal Scrap #{id - GameLocation.JOURNAL_INDEX}"
                        )
                        && (id != 10 || player.mailReceived.Contains("QiChallengeComplete"))
                )
                .Select(id => id.ToString("G"));
        }
    }
}
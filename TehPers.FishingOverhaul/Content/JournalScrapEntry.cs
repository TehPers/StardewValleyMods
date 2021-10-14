using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Content
{
    public record JournalScrapEntry(AvailabilityInfo AvailabilityInfo) : TrashEntry(
        NamespacedKey.SdvObject(842),
        AvailabilityInfo
    )
    {
        public override bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        )
        {
            // Choose a note ID
            var notesInfo = Game1.content.Load<Dictionary<int, string>>(@"Data\SecretNotes");
            var chosenNote = notesInfo.Keys.Where(id => id >= GameLocation.JOURNAL_INDEX)
                .Except(fishingInfo.User.secretNotesSeen)
                .Where(
                    id => !fishingInfo.User.hasItemInInventoryNamed(
                        $"Journal Scrap #{id - GameLocation.JOURNAL_INDEX}"
                    )
                )
                .OrderBy(id => id)
                .Select(id => (int?)id)
                .FirstOrDefault();

            // Make sure a note was chosen
            if (chosenNote is not { } chosenNoteId)
            {
                item = default;
                return false;
            }

            // Create the note
            var note = new Object(842, 1);
            note.name = $"{note.name} #{chosenNoteId}";

            item = new(note);
            return true;
        }
    }
}
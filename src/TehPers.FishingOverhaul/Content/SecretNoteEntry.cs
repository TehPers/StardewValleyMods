using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Extensions;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Content
{
    internal record SecretNoteEntry(AvailabilityInfo AvailabilityInfo) : TrashEntry(
        NamespacedKey.SdvObject(79),
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
            var chosenNote = notesInfo.Keys.Where(id => id < GameLocation.JOURNAL_INDEX)
                .Except(fishingInfo.User.secretNotesSeen)
                .Where(
                    id => !fishingInfo.User.Items.Any(item => item.Name.Equals(
                            $"Secret Note #{id - GameLocation.JOURNAL_INDEX}"
                        ))
                        && (id != 10 || fishingInfo.User.mailReceived.Contains("QiChallengeComplete"))
                )
                .ToWeighted(_ => 1)
                .ChooseOrDefault(Game1.random);

            // Make sure a note was chosen
            if (chosenNote is not { Value: var chosenNoteId })
            {
                item = default;
                return false;
            }

            // Create the note
            var note = new SObject("79", 1);
            note.name = $"{note.name} #{chosenNoteId}";

            item = new(note);
            return true;
        }
    }
}

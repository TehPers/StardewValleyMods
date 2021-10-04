using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Content
{
    public class FishingContent
    {
        public IManifest Mod { get; }
        public Dictionary<NamespacedKey, FishTraits> Traits { get; }
        public List<FishEntry> FishEntries { get; }
        public List<TrashEntry> TrashEntries { get; }
        public List<TreasureEntry> TreasureEntries { get; }

        public FishingContent(
            IManifest mod,
            Dictionary<NamespacedKey, FishTraits>? traits,
            List<FishEntry>? fishEntries,
            List<TrashEntry>? trashEntries,
            List<TreasureEntry>? treasureEntries
        )
        {
            this.Mod = mod;
            this.Traits = traits ?? new();
            this.FishEntries = fishEntries ?? new();
            this.TrashEntries = trashEntries ?? new();
            this.TreasureEntries = treasureEntries ?? new();
        }
    }
}
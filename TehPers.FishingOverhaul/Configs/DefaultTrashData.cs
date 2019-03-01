using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Structs;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class DefaultTrashData : ITrashData {
        private const int MIN_TRASH_ID = 167;
        private const int MAX_TRASH_ID = 173;
        public IEnumerable<int> PossibleIds { get; } = Enumerable.Range(DefaultTrashData.MIN_TRASH_ID, DefaultTrashData.MAX_TRASH_ID - DefaultTrashData.MIN_TRASH_ID);

        public bool MeetsCriteria(Farmer who, string locationName, WaterTypes waterTypes, SDateTime dateTime, Weather weather, int fishingLevel, int? mineLevel) {
            return locationName != "Submarine";
        }

        public double GetWeight() {
            return DefaultTrashData.MAX_TRASH_ID - DefaultTrashData.MIN_TRASH_ID;
        }
    }
}
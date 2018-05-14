using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class SpecificTrashData : ITrashData {
        public IEnumerable<int> PossibleIds { get; set; }
        public string Location { get; }
        public WaterType WaterType { get; }
        public Season Season { get; }
        public Weather Weather { get; }
        public int FishingLevel { get; }
        public int? MineLevel { get; }
        public double Weight { get; }

        public SpecificTrashData(IEnumerable<int> ids, double weight, string location, WaterType waterType = WaterType.Both, Season season = Season.Spring | Season.Summer | Season.Fall | Season.Winter, Weather weather = Weather.Sunny | Weather.Rainy, int fishingLevel = 0, int? mineLevel = null) {
            this.PossibleIds = ids.ToArray();
            this.Weight = weight;
            this.Location = location;
            this.WaterType = waterType;
            this.Season = season;
            this.Weather = weather;
            this.FishingLevel = fishingLevel;
            this.MineLevel = mineLevel;
        }

        public bool MeetsCriteria(Farmer who, string locationName, WaterType waterType, Season season, Weather weather, int time, int fishingLevel, int? mineLevel) {
            return (this.Location == null || locationName == this.Location)
                   && (waterType & this.WaterType) != 0
                   && (season & this.Season) != 0
                   && (weather & this.Weather) != 0
                   && fishingLevel >= this.FishingLevel
                   && (this.MineLevel == null || mineLevel == this.MineLevel);
        }

        public double GetWeight() {
            return 1;
        }
    }
}
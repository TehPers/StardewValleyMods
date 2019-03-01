using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Structs;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class SpecificTrashData : ITrashData {
        public IEnumerable<int> PossibleIds { get; set; }
        public string Location { get; }
        public WaterTypes WaterTypes { get; }
        public Season Season { get; }
        public Weather Weather { get; }
        public int FishingLevel { get; }
        public int? MineLevel { get; }
        public bool InvertLocations { get; }
        public double Weight { get; }

        public SpecificTrashData(IEnumerable<int> ids, double weight, string location, WaterTypes waterTypes = WaterTypes.Any, Season season = Season.Any, Weather weather = Weather.Sunny | Weather.Rainy, int fishingLevel = 0, int? mineLevel = null, bool invertLocations = false) {
            this.PossibleIds = ids.ToArray();
            this.Weight = weight;
            this.Location = location;
            this.WaterTypes = waterTypes;
            this.Season = season;
            this.Weather = weather;
            this.FishingLevel = fishingLevel;
            this.MineLevel = mineLevel;
            this.InvertLocations = invertLocations;
        }

        public bool MeetsCriteria(Farmer who, string locationName, WaterTypes waterTypes, SDateTime dateTime, Weather weather, int fishingLevel, int? mineLevel) {
            return (this.InvertLocations ^ (this.Location == null || locationName == this.Location))
                   && (this.WaterTypes & waterTypes) != 0
                   && (this.Season & dateTime.Season) != 0
                   && (this.Weather & weather) != 0
                   && this.FishingLevel <= fishingLevel
                   && (this.MineLevel == null || this.MineLevel == mineLevel);
        }

        public double GetWeight() {
            return this.Weight;
        }
    }
}
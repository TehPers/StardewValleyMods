using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.Core.Helpers.Static;
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
        public bool InvertLocations { get; }
        public double Weight { get; }

        public SpecificTrashData(IEnumerable<int> ids, double weight, string location, WaterType waterType = WaterType.Both, Season season = Season.Spring | Season.Summer | Season.Fall | Season.Winter, Weather weather = Weather.Sunny | Weather.Rainy, int fishingLevel = 0, int? mineLevel = null, bool invertLocations = false) {
            PossibleIds = ids.ToArray();
            Weight = weight;
            Location = location;
            WaterType = waterType;
            Season = season;
            Weather = weather;
            FishingLevel = fishingLevel;
            MineLevel = mineLevel;
            InvertLocations = invertLocations;
        }

        public bool MeetsCriteria(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel) {
            if (string.Equals(locationName, "BeachNightMarket", StringComparison.OrdinalIgnoreCase)) {
                locationName = "Beach";
            }
            return (InvertLocations ^ (
                        Location == null ||
                        locationName == Location ||
                        (Location == "UndergroundMines" && mineLevel > 0 && (MineLevel == null || MineLevel == mineLevel)))
                   )
                   && !(InvertLocations && locationName == "Submarine") /* MBD: Crappy hack to stop catching green algae on the sub.  Better solution would be to replace invertLocations with a list of blocked locations */
                   && (WaterType & waterType) != 0
                   && (Season & date.GetSeason()) != 0
                   && (Weather & weather) != 0
                   && FishingLevel <= fishingLevel;
        }

        public double GetWeight() {
            return Weight;
        }
    }
}
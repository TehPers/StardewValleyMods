using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.Core.Helpers.Static;
using TehPers.FishingOverhaul.Api;

namespace TehPers.MoreFish {
    public class FishData : IFishData {
        public float Weight { get; set; }
        public List<TimeInterval> Times { get; }
        public int MinLevel { get; set; }
        public WaterType WaterType { get; set; }
        public Season Season { get; set; }
        public Weather Weather { get; set; }
        public int? MineLevel { get; set; }

        public FishData(float weight, IEnumerable<TimeInterval> times, int minLevel = 0, WaterType? waterType = null, Season? season = null, Weather? weather = null, int? mineLevel = null) {
            this.Weight = weight;
            this.Times = new List<TimeInterval>(times);
            this.MinLevel = minLevel;
            this.WaterType = waterType ?? WaterType.Both;
            this.Season = season ?? Season.Spring | Season.Summer | Season.Fall | Season.Winter;
            this.Weather = weather ?? Weather.Rainy | Weather.Sunny;
            this.MineLevel = mineLevel;
        }

        public float GetWeight(Farmer who) {
            return this.Weight;
        }

        public bool MeetsCriteria(int fish, WaterType waterType, SDate date, Weather weather, int time, int level, int? mineLevel) {
            return this.Times.Any(interval => time >= interval.Start && time < interval.Finish)
                   && this.MinLevel <= level
                   && (this.WaterType & waterType) > 0
                   && (this.Season & date.GetSeason()) > 0
                   && (this.Weather & weather) > 0
                   && (this.MineLevel == null || this.MineLevel == mineLevel);
        }
    }
}
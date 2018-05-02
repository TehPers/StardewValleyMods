using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehCore.Configs;
using TehCore.Enums;

namespace FishingOverhaul.Configs {
    [JsonDescribe]
    public class FishData {

        [Description("The weighted chance of this fish appearing")]
        public double Chance { get; set; }

        [Description("The times this fish can appear")]
        public List<TimeInterval> Times { get; } = new List<TimeInterval>();

        [Description("The types of water this fish can appear in. Lake = 1, river = 2. For both, add the numbers together.")]
        public WaterType WaterType { get; set; }

        [Description("The seasons this fish can appear in. Spring = 1, summer = 2, fall = 4, winter = 8. For multiple seasons, add the numbers together.")]
        public Season Season { get; set; }

        [Description("The weather this fish can appear in. Sunny = 1, rainy = 2. For both, add the numbers together.")]
        public Weather Weather { get; set; }

        [Description("WIP - Not really sure what this does.")]
        public int MinCastDistance { get; set; }

        [Description("The minimum fishing level required to find this fish.")]
        public int MinLevel { get; set; }

        [Description("The mine level this fish can be found on.")]
        public int MineLevel { get; set; }

        public FishData(double chance, WaterType waterType, Season season, int minTime = 600, int maxTime = 2600, int minDepth = 0, int minLevel = 0, Weather? weather = null, int mineLevel = -1) {
            this.Chance = chance;
            this.WaterType = waterType;
            this.Season = season;
            this.Times.Add(new TimeInterval(minTime, maxTime));
            this.MinCastDistance = minDepth;
            this.MinLevel = minLevel;
            this.Weather = weather ?? Weather.Sunny | Weather.Rainy;
            this.MineLevel = mineLevel;
        }

        public bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int depth, int level) {
            return (this.WaterType & waterType) > 0 && (this.Season & season) > 0 && (this.Weather & weather) > 0 && depth >= this.MinCastDistance && level >= this.MinLevel && this.Times.Any(range => time >= range.Start && time <= range.Finish);
        }

        public bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int depth, int level, int mineLevel) {
            return this.MeetsCriteria(waterType, season, weather, time, depth, level) && (this.MineLevel == -1 || mineLevel == this.MineLevel);
        }

        public virtual float GetWeightedChance(int depth, int level) {
            if (this.MinCastDistance >= 5) return (float) this.Chance + level / 50f;
            return (float) (5 - depth) / (5 - this.MinCastDistance) * (float) this.Chance + level / 50f;
        }

        public override string ToString() => $"Chance: {this.Chance}, Weather: {this.Weather}, Season: {this.Season}";

        [JsonDescribe]
        public struct TimeInterval {

            [Description("The earliest time in this interval")]
            public int Start;

            [Description("The latest time in this interval")]
            public int Finish;

            public TimeInterval(int start, int finish) {
                this.Start = start;
                this.Finish = finish;
            }
        }
    }
}

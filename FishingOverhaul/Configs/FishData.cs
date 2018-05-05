using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TehCore;
using TehCore.Configs;
using TehCore.Enums;
using TehCore.Weighted;

namespace FishingOverhaul.Configs {

    [JsonDescribe]
    public class FishData {
        [Description("The weighted chance of this fish appearing")]
        public double Chance { get; set; }

        [Description("The times this fish can appear")]
        public HashSet<TimeInterval> Times { get; } = new HashSet<TimeInterval>();

        [Description("The types of water this fish can appear in. Lake = 1, river = 2. For both, add the numbers together.")]
        public WaterType WaterType { get; set; }

        [Description("The seasons this fish can appear in. Spring = 1, summer = 2, fall = 4, winter = 8. For multiple seasons, add the numbers together.")]
        public Season Season { get; set; }

        [Description("The weather this fish can appear in. Sunny = 1, rainy = 2. For both, add the numbers together.")]
        public Weather Weather { get; set; }

        [Description("The minimum fishing level required to find this fish.")]
        public int MinLevel { get; set; }

        [Description("The mine level this fish can be found on.")]
        public int MineLevel { get; set; }

        public FishData(double chance, int minTime, int maxTime, WaterType waterType, Season season, int minLevel = 0, Weather? weather = null, int mineLevel = -1)
            : this(chance, new[] { new TimeInterval(minTime, maxTime) }, waterType, season, minLevel, weather, mineLevel) { }

        public FishData(double chance, IEnumerable<TimeInterval> times, WaterType waterType, Season season, int minLevel = 0, Weather? weather = null, int mineLevel = -1) {
            this.Chance = chance;
            this.WaterType = waterType;
            this.Season = season;
            this.MinLevel = minLevel;
            this.Weather = weather ?? Weather.Sunny | Weather.Rainy;
            this.MineLevel = mineLevel;
            if (times != null)
                this.Times = new HashSet<TimeInterval>(times);
        }

        public bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int level) {
            // Note: HasFlag won't work because these are checking for an intersection, not for a single bit
            return (this.WaterType & waterType) > 0
                   && (this.Season & season) > 0
                   && (this.Weather & weather) > 0
                   && level >= this.MinLevel
                   && this.Times.Any(t => time >= t.Start && time < t.Finish);
        }

        public bool MeetsCriteria(WaterType waterType, Season season, Weather weather, int time, int level, int mineLevel) {
            return this.MeetsCriteria(waterType, season, weather, time, level)
                   && (this.MineLevel == -1 || mineLevel == this.MineLevel);
        }

        public virtual float GetWeightedChance(int level) {
            return (float) this.Chance + level / 50f;
        }

        public override string ToString() => $"Chance: {this.Chance}, Weather: {this.Weather}, Season: {this.Season}";

        public static FishData Trash { get; } = new FishData(1, 600, 2600, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter);

        public class CombinedFishData : IWeighted {
            public int Fish { get; }
            public FishData Data { get; }
            public int Level { get; }

            public CombinedFishData(int fish, FishData data, int level) {
                this.Fish = fish;
                this.Data = data;
                this.Level = level;
            }

            public double GetWeight() {
                return this.Data.GetWeightedChance(this.Level);
            }
        }

        [JsonDescribe]
        public struct TimeInterval {
            [Description("The earliest time in this interval")]
            public int Start { get; }

            [Description("The latest time in this interval")]
            public int Finish { get; }

            [JsonConstructor]
            public TimeInterval(int start, int finish) {
                this.Start = start;
                this.Finish = finish;
            }

            public override bool Equals(object obj) {
                return base.Equals(obj);
            }

            public bool Equals(TimeInterval other) {
                return this.Start == other.Start && this.Finish == other.Finish;
            }

            public override int GetHashCode() {
                unchecked {
                    return (this.Start * 397) ^ this.Finish;
                }
            }
        }
    }
}

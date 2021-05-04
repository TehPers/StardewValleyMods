using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Json.Serialization;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {

    [JsonDescribe]
    public class FishData : IFishData {
        [Description("The weighted chance of this fish appearing")]
        public double Chance { get; set; } = 1D;

        [Description("The times this fish can appear")]
        public List<TimeInterval> Times { get; set; } = new List<TimeInterval>();

        [Description("The types of water this fish can appear in. Lake = 1, river = 2. For both, add the numbers together.")]
        public WaterType WaterType { get; set; } = WaterType.Both;

        [Description("The seasons this fish can appear in. Spring = 1, summer = 2, fall = 4, winter = 8. For multiple seasons, add the numbers together.")]
        public Season Season { get; set; } = Season.Spring | Season.Summer | Season.Fall | Season.Winter;

        [Description("The weather this fish can appear in. Sunny = 1, rainy = 2. For both, add the numbers together.")]
        public Weather Weather { get; set; } = Weather.Rainy | Weather.Sunny;

        [Description("The minimum fishing level required to find this fish.")]
        public int MinLevel { get; set; }

        [Description("The mine level this fish can be found on, or null if it can be found on any floor.")]
        public int? MineLevel { get; set; }

        public FishData() { }

        public FishData(double chance, int minTime, int maxTime, WaterType waterType, Season season, int minLevel = 0, Weather? weather = null, int? mineLevel = null)
            : this(chance, new[] { new TimeInterval(minTime, maxTime) }, waterType, season, minLevel, weather, mineLevel) { }

        public FishData(double chance, IEnumerable<TimeInterval> times, WaterType waterType, Season season, int minLevel = 0, Weather? weather = null, int? mineLevel = null) {
            Chance = chance;
            WaterType = waterType;
            Season = season;
            MinLevel = minLevel;
            Weather = weather ?? Weather.Sunny | Weather.Rainy;
            MineLevel = mineLevel;
            if (times != null) {
                Times = new List<TimeInterval>(times);
            }
        }

        public bool MeetsCriteria(int fish, WaterType waterType, SDate date, Weather weather, int time, int level) {
            // Note: HasFlag won't work because these are checking for an intersection, not for a single bit
            return (WaterType & waterType) > 0
                   && (Season & date.GetSeason()) > 0
                   && (Weather & weather) > 0
                   && level >= MinLevel
                   && Times.Any(t => time >= t.Start && time < t.Finish);
        }

        public bool MeetsCriteria(int fish, WaterType waterType, SDate date, Weather weather, int time, int level, int? mineLevel) {
            return MeetsCriteria(fish, waterType, date, weather, time, level)
                   && (MineLevel == null || mineLevel == MineLevel);
        }

        public virtual float GetWeight(Farmer who) {
            return (float) Chance + who.FishingLevel / 50f;
        }

        public override string ToString() => $"Chance: {Chance}, Weather: {Weather}, Season: {Season}";

        public static IFishData Trash { get; } = new FishData(1, 600, 2600, WaterType.Both, Season.Spring | Season.Summer | Season.Fall | Season.Winter);

        [JsonDescribe]
        public class TimeInterval {
            [Description("The earliest time in this interval")]
            public int Start { get; set; }

            [Description("The latest time in this interval")]
            public int Finish { get; set; }

            public TimeInterval() { }

            public TimeInterval(int start, int finish) {
                Start = start;
                Finish = finish;
            }
        }
    }
}

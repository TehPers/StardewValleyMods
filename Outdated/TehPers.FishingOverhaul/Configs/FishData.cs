using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Json;
using TehPers.CoreMod.Api.Structs;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {

    [JsonDescribe]
    public class FishData : IFishData {
        [Description("The weighted chance of this fish appearing")]
        public double Chance { get; set; } = 1D;

        [Description("The times this fish can appear")]
        public List<TimeInterval> Times { get; set; } = new List<TimeInterval>();

        [Description("The types of water this fish can appear in. Lake = 1, river = 2. For both, add the numbers together.")]
        public WaterTypes WaterTypes { get; set; } = WaterTypes.Any;

        [Description("The seasons this fish can appear in. Spring = 1, summer = 2, fall = 4, winter = 8. For multiple seasons, add the numbers together.")]
        public Season Season { get; set; } = Season.Spring | Season.Summer | Season.Fall | Season.Winter;

        [Description("The weather this fish can appear in. Sunny = 1, rainy = 2. For both, add the numbers together.")]
        public Weather Weather { get; set; } = Weather.Rainy | Weather.Sunny;

        [Description("The minimum fishing level required to find this fish.")]
        public int MinLevel { get; set; }

        [Description("The mine level this fish can be found on, or null if it can be found on any floor.")]
        public int? MineLevel { get; set; }

        public FishData() { }

        public FishData(double chance, int minTime, int maxTime, WaterTypes waterTypes, Season season, int minLevel = 0, Weather? weather = null, int? mineLevel = null)
            : this(chance, new[] { new TimeInterval(minTime, maxTime) }, waterTypes, season, minLevel, weather, mineLevel) { }

        public FishData(double chance, IEnumerable<TimeInterval> times, WaterTypes waterTypes, Season season, int minLevel = 0, Weather? weather = null, int? mineLevel = null) {
            this.Chance = chance;
            this.WaterTypes = waterTypes;
            this.Season = season;
            this.MinLevel = minLevel;
            this.Weather = weather ?? Weather.Sunny | Weather.Rainy;
            this.MineLevel = mineLevel;
            if (times != null) {
                this.Times = new List<TimeInterval>(times);
            }
        }

        public bool MeetsCriteria(int fish, WaterTypes waterTypes, SDateTime dateTime, Weather weather, int level) {
            // Note: HasFlag won't work because these are checking for an intersection, not for a single bit
            return this.WaterTypes.HasAnyFlags(waterTypes)
                   && this.Season.HasAnyFlags(dateTime.Season)
                   && this.Weather.HasAnyFlags(weather)
                   && level >= this.MinLevel
                   && this.Times.Any(t => dateTime.TimeOfDay >= t.Start && dateTime.TimeOfDay < t.Finish);
        }

        public bool MeetsCriteria(int fish, WaterTypes waterTypes, SDateTime dateTime, Weather weather, int level, int? mineLevel) {
            return this.MeetsCriteria(fish, waterTypes, dateTime, weather, level)
                   && (this.MineLevel == null || mineLevel == this.MineLevel);
        }

        public virtual float GetWeight(Farmer who) {
            return (float) this.Chance + who.FishingLevel / 50f;
        }

        public override string ToString() => $"Chance: {this.Chance}, Weather: {this.Weather}, Season: {this.Season}";

        public static IFishData Trash { get; } = new FishData(1, 600, 2600, WaterTypes.Any, Season.Any);

        [JsonDescribe]
        public class TimeInterval {
            [Description("The earliest time in this interval")]
            public int Start { get; set; }

            [Description("The latest time in this interval")]
            public int Finish { get; set; }

            public TimeInterval() { }

            public TimeInterval(int start, int finish) {
                this.Start = start;
                this.Finish = finish;
            }
        }
    }
}

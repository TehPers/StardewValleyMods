using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using StardewValley;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    public class FishingChances : IFishingChances
    {
        [Description("The base chance. Total chance is calculated as locationFactor * (baseChance + streak * streakFactor + dailyLuck * dailyLuckFactor + luckLevel * luckLevelFactor).")]
        public double BaseChance { get; set; }
        
        [Description("The effect that streak has on this chance.")]
        public double StreakFactor { get; set; }
        
        [Description("The effect that daily luck has on this chance.")]
        public double DailyLuckFactor { get; set; }
        
        [Description("The effect that luck level has on this chance.")]
        public double LuckLevelFactor { get; set; }

        [Description("The effects that specific locations have on this chance. This effect applies all the other factors, so it has a much larger effect on the chance.")]
        [JsonProperty]
        public Dictionary<GameLocation, double> LocationFactors { get; set; } = new Dictionary<GameLocation, double>();

        [JsonIgnore]
        IDictionary<GameLocation, double> IFishingChances.LocationFactors => this.LocationFactors;
    }
}
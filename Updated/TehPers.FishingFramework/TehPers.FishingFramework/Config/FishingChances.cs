using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using StardewValley;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Config
{
    /// <inheritdoc cref="IFishingChances" />
    [JsonDescribe]
    public class FishingChances : IFishingChances
    {
        /// <summary>
        /// Gets or sets the base chance. Total chance is calculated as locationFactor * (baseChance + sum(factor * thing it's a factor of)), bounded in the range [minChance, maxChance], then bounded once again in the range [0, 1].
        /// </summary>
        [Description("The base chance. Total chance is calculated as locationFactor * (baseChance + sum(factor * thing it's a factor of)), bounded in the range [minChance, maxChance], then bounded once again in the range [0, 1].")]
        public virtual double BaseChance { get; set; }

        /// <summary>
        /// Gets or sets the effect that streak has on this chance.
        /// </summary>
        [Description("The effect that streak has on this chance.")]
        public double StreakFactor { get; set; }

        /// <summary>
        /// Gets or sets the effect that fishing level has on this chance.
        /// </summary>
        [Description("The effect that fishing level has on this chance.")]
        public double FishingLevelFactor { get; set; }

        /// <summary>
        /// Gets or sets the effect that daily luck has on this chance.
        /// </summary>
        [Description("The effect that daily luck has on this chance.")]
        public double DailyLuckFactor { get; set; }

        /// <summary>
        /// Gets or sets the effect that luck level has on this chance.
        /// </summary>
        [Description("The effect that luck level has on this chance.")]
        public double LuckLevelFactor { get; set; }

        /// <summary>
        /// Gets or sets the effects that specific locations have on this chance.
        /// </summary>
        [Description("The effects that specific locations have on this chance. This effect applies after all the other factors, so it has a much larger effect on the chance. Keys are location names and values are their multipliers.")]
        [JsonProperty]
        public Dictionary<GameLocation, double> LocationFactors { get; set; } = new Dictionary<GameLocation, double>();

        /// <inheritdoc />
        [JsonIgnore]
        IReadOnlyDictionary<GameLocation, double> IFishingChances.LocationFactors => this.LocationFactors;

        /// <summary>
        /// Gets or sets the minimum possible chance.
        /// </summary>
        [Description("The minimum possible chance.")]
        public double MinChance { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum possible chance.
        /// </summary>
        [Description("The maximum possible chance.")]
        public double MaxChance { get; set; } = 1;
    }
}
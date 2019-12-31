using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using StardewValley;
using TehPers.Core.Api;
using TehPers.Core.Api.Chrono;
using TehPers.Core.Api.Extensions;

namespace TehPers.FishingFramework.Api
{
    /// <inheritdoc />
    public class FishAvailability : IFishAvailability
    {
        [JsonIgnore]
        private readonly STimeInterval[] times;

        /// <inheritdoc />
        [Description("The ID of the fish.")]
        public NamespacedId FishId { get; }

        /// <summary>
        /// Gets the weighted chance of this fish appearing.
        /// </summary>
        [Description("The weighted chance of this fish appearing.")]
        public double Chance { get; }

        /// <summary>
        /// Gets the times this fish can appear.
        /// </summary>
        [Description("The times this fish can appear.")]
        public IEnumerable<STimeInterval> Times => this.times;

        /// <summary>
        /// Gets the name of the game location this fish can appear in.
        /// </summary>
        [Description("The name of the game location this fish can appear in.")]
        public string Location { get; }

        /// <summary>
        /// Gets the water types this fish can appear in.
        /// </summary>
        [Description("The types of water this fish can appear in. Lake = 1, river = 2. For both, add the numbers together.")]
        public WaterTypes WaterTypes { get; }

        /// <summary>
        /// Gets the seasons this fish can appear in.
        /// </summary>
        [Description("The seasons this fish can appear in. Spring = 1, summer = 2, fall = 4, winter = 8. For multiple seasons, add the numbers together.")]
        public Seasons Seasons { get; }

        /// <summary>
        /// Gets the weather this fish can appear in.
        /// </summary>
        [Description("The weather this fish can appear in. Sunny = 1, rainy = 2. For both, add the numbers together.")]
        public Weathers Weathers { get; }

        /// <summary>
        /// Gets the minimum fishing level required for this fish to appear.
        /// </summary>
        [Description("The minimum fishing level required to find this fish.")]
        public int MinLevel { get; }

        /// <summary>
        /// Gets the mine level this fish can be found on in the mines or null if it can be found on any floor.
        /// </summary>
        [Description("The mine level this fish can be found on in the mines or null if it can be found on any floor.")]
        public int? MineLevel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FishAvailability"/> class.
        /// </summary>
        /// <param name="fishId">The <see cref="NamespacedId"/> for the fish.</param>
        /// <param name="chance">The weighted chance of this fish appearing.</param>
        /// <param name="times">The times this fish can appear.</param>
        /// <param name="location">The name of the game location this fish can appear in.</param>
        /// <param name="seasons">The seasons this fish can appear in.</param>
        /// <param name="weathers">The weathers this fish can appear in.</param>
        /// <param name="waterTypes">The water types this fish can appear in.</param>
        /// <param name="minLevel">The minimum fishing level required for this fish to appear.</param>
        /// <param name="mineLevel">The mine level this fish can be found on in the mines.</param>
        [JsonConstructor]
        public FishAvailability(
            NamespacedId fishId,
            double chance,
            STimeInterval[] times,
            string location,
            Seasons seasons = Seasons.Any,
            Weathers weathers = Weathers.Any,
            WaterTypes waterTypes = WaterTypes.Any,
            int minLevel = 0,
            int? mineLevel = null)
        {
            this.FishId = fishId;
            this.Chance = chance;
            this.times = times ?? throw new ArgumentNullException(nameof(times));
            this.Location = location ?? throw new ArgumentNullException(nameof(location));
            this.Seasons = seasons;
            this.Weathers = weathers;
            this.WaterTypes = waterTypes;
            this.MinLevel = minLevel;
            this.MineLevel = mineLevel;
        }

        /// <inheritdoc />
        public double GetWeightedChance(Farmer who, GameLocation location, Weathers weather, WaterTypes water, SDateTime dateTime, int? mineLevel = null)
        {
            _ = location ?? throw new ArgumentNullException(nameof(location));
            _ = who ?? throw new ArgumentNullException(nameof(who));

            if (!this.MeetsCriteria(location, water, dateTime, weather, who.FishingLevel, mineLevel))
            {
                return 0;
            }

            return (float)this.Chance + who.FishingLevel / 50f;
        }

        private bool MeetsCriteria(GameLocation location, WaterTypes waterTypes, SDateTime dateTime, Weathers weather, int level, int? mineLevel)
        {
            // Note: HasFlag won't work because these are checking for an intersection, not for a single bit
            return this.Location.Equals(location.Name, StringComparison.Ordinal)
                   && this.WaterTypes.HasFlags(waterTypes)
                   && this.Seasons.HasFlags(dateTime.Season)
                   && this.Weathers.HasFlags(weather)
                   && level >= this.MinLevel
                   && this.Times.Any(t => dateTime.TimeOfDay >= t.Start && dateTime.TimeOfDay < t.Finish)
                   && (this.MineLevel == null || mineLevel == this.MineLevel);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Chance: {this.Chance}, Weather: {this.Weathers}, Season: {this.Seasons}";
        }
    }
}
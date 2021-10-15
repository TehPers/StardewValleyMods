using System;
using System.Collections.Generic;
using System.ComponentModel;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for the chances of catching something while fishing.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
    [JsonDescribe]
    public class FishingChances : IModConfig
    {
        /// <summary>
        /// The base chance. Total chance is calculated as
        /// locationFactor * (baseChance + sum(factor * thing it's a factor of)), bounded in the
        /// range [minChance, maxChance], then bounded once again in the range [0, 1].
        /// </summary>
        [DefaultValue(0d)]
        public double BaseChance { get; set; }

        /// <summary>
        /// The effect that streak has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double StreakFactor { get; set; }

        /// <summary>
        /// The effect that fishing level has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double FishingLevelFactor { get; set; }

        /// <summary>
        /// The effect that daily luck has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double DailyLuckFactor { get; set; }

        /// <summary>
        /// The effect that luck level has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double LuckLevelFactor { get; set; }

        /// <summary>
        /// The minimum possible chance.
        /// </summary>
        [DefaultValue(0d)]
        public double MinChance { get; set; }

        /// <summary>
        /// The maximum possible chance.
        /// </summary>
        [DefaultValue(1d)]
        public double MaxChance { get; set; } = 1d;

        public virtual void Reset()
        {
            this.BaseChance = default;
            this.StreakFactor = default;
            this.FishingLevelFactor = default;
            this.DailyLuckFactor = default;
            this.LuckLevelFactor = default;
            this.MinChance = 0d;
            this.MaxChance = 1d;
            this.LocationFactors.Clear();
        }

        /// <summary>
        /// The effects that specific locations have on this chance. Keys are location names and
        /// values are their factors.
        /// </summary>
        public Dictionary<string, double> LocationFactors { get; set; } = new();

        public virtual void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.chances.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.chances.{key}.desc");

            configApi.RegisterClampedOption(
                manifest,
                Name("baseChance"),
                Desc("baseChance"),
                () => (float)this.BaseChance,
                val => this.BaseChance = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("streakFactor"),
                Desc("streakFactor"),
                () => (float)this.StreakFactor,
                val => this.StreakFactor = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("fishingLevelFactor"),
                Desc("fishingLevelFactor"),
                () => (float)this.FishingLevelFactor,
                val => this.FishingLevelFactor = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("dailyLuckFactor"),
                Desc("dailyLuckFactor"),
                () => (float)this.DailyLuckFactor,
                val => this.DailyLuckFactor = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("luckLevelFactor"),
                Desc("luckLevelFactor"),
                () => (float)this.LuckLevelFactor,
                val => this.LuckLevelFactor = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("minChance"),
                Desc("minChance"),
                () => (float)this.MinChance,
                val => this.MinChance = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("maxChance"),
                Desc("maxChance"),
                () => (float)this.MaxChance,
                val => this.MaxChance = val,
                0f,
                1f
            );
            configApi.RegisterParagraph(manifest, Desc("locationFactors"));
        }

        protected virtual double GetUnclampedChance(Farmer farmer, int streak)
        {
            // Base chance
            var chance = this.BaseChance;

            // Luck
            chance += farmer.LuckLevel * this.LuckLevelFactor;
            chance += farmer.DailyLuck * this.DailyLuckFactor;

            // Stats
            chance += farmer.FishingLevel * this.FishingLevelFactor;

            // Streak
            chance += streak * this.StreakFactor;

            // Location
            if (farmer.currentLocation is { Name: { } locationName }
                && this.LocationFactors.TryGetValue(locationName, out var factor))
            {
                chance += factor;
            }

            return chance;
        }

        /// <summary>
        /// Calculates the specific chance for a farmer.
        /// </summary>
        /// <param name="farmer">The farmer to calculate the chance for.</param>
        /// <param name="streak">The farmer's fishing streak.</param>
        /// <returns>The calculated chance for that farmer.</returns>
        public double GetChance(Farmer farmer, int streak)
        {
            return Math.Min(
                this.MaxChance,
                Math.Max(this.MinChance, this.GetUnclampedChance(farmer, streak))
            );
        }
    }
}
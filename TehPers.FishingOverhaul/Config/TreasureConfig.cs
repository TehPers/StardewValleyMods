﻿using System.ComponentModel;
using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for treasure.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
    [JsonDescribe]
    public sealed class TreasureConfig : IModConfig
    {
        /// <summary>
        /// Maximum amount of treasure you can find in a single chest while fishing.
        /// </summary>
        [DefaultValue(3)]
        public int MaxTreasureQuantity { get; set; } = 3;

        /// <summary>
        /// Whether the treasure randomizer should be allowed to select the same loot option
        /// multiple times. Some loot can't be repeated even if duplicate loot is enabled.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowDuplicateLoot { get; set; } = true;

        /// <summary>
        /// Affects how fast you catch treasure.
        /// </summary>
        [DefaultValue(1f)]
        public float CatchSpeed { get; set; } = 1f;

        /// <summary>
        /// Affects how fast the treasure bar drains when the bobber isn't on the chest.
        /// </summary>
        [DefaultValue(1f)]
        public float DrainSpeed { get; set; } = 1f;

        /// <summary>
        /// The chances of finding treasure while fishing.
        /// </summary>
        public TreasureChances TreasureChances { get; init; } = new()
        {
            BaseChance = 0.15d,
            StreakFactor = 0.01d,
            FishingLevelFactor = 0d,
            DailyLuckFactor = 0.5d,
            LuckLevelFactor = 0.005d,
            MaxChance = 0.5d,
        };

        /// <summary>
        /// The chances of getting additional loot when finding treasure. This is rolled until it
        /// fails or the max number of items have been added to the loot.
        /// </summary>
        public FishingChances AdditionalLootChances { get; init; } = new()
        {
            BaseChance = 0.5d,
            StreakFactor = 0.1d,
            FishingLevelFactor = 0d,
            DailyLuckFactor = 0.5d,
            LuckLevelFactor = 0.005d,
            MaxChance = 0.8d,
        };

        void IModConfig.Reset()
        {
            this.MaxTreasureQuantity = 3;
            this.AllowDuplicateLoot = true;
            this.CatchSpeed = 1f;
            this.DrainSpeed = 1f;

            // Treasure chances
            this.TreasureChances.BaseChance = 0.15d;
            this.TreasureChances.StreakFactor = 0.01d;
            this.TreasureChances.FishingLevelFactor = 0d;
            this.TreasureChances.DailyLuckFactor = 0.5d;
            this.TreasureChances.LuckLevelFactor = 0.005d;
            this.TreasureChances.MaxChance = 0.5d;

            // Additional loot chances
            this.AdditionalLootChances.BaseChance = 0.5d;
            this.AdditionalLootChances.StreakFactor = 0.1d;
            this.AdditionalLootChances.FishingLevelFactor = 0d;
            this.AdditionalLootChances.DailyLuckFactor = 0.5d;
            this.AdditionalLootChances.LuckLevelFactor = 0.005d;
            this.AdditionalLootChances.MaxChance = 0.8d;
        }

        void IModConfig.RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.treasure.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.treasure.{key}.desc");

            configApi.AddNumberOption(
                manifest,
                () => this.MaxTreasureQuantity,
                val => this.MaxTreasureQuantity = val,
                () => Name("maxTreasureQuantity"),
                () => Desc("maxTreasureQuantity"),
                0,
                36
            );
            configApi.AddBoolOption(
                manifest,
                () => this.AllowDuplicateLoot,
                val => this.AllowDuplicateLoot = val,
                () => Name("allowDuplicateLoot"),
                () => Desc("allowDuplicateLoot")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.CatchSpeed,
                val => this.CatchSpeed = val,
                () => Name("catchSpeed"),
                () => Desc("catchSpeed"),
                0f,
                3f
            );
            configApi.AddNumberOption(
                manifest,
                () => this.DrainSpeed,
                val => this.DrainSpeed = val,
                () => Name("drainSpeed"),
                () => Desc("drainSpeed"),
                0f,
                3f
            );

            // Treasure chances
            configApi.AddSectionTitle(
                manifest,
                () => Name("treasureChances"),
                () => Desc("treasureChances")
            );
            this.TreasureChances.RegisterOptions(configApi, manifest, translations);

            // Additional loot chances
            configApi.AddSectionTitle(
                manifest,
                () => Name("additionalLootChances"),
                () => Desc("additionalLootChances")
            );
            this.AdditionalLootChances.RegisterOptions(configApi, manifest, translations);
        }
    }
}
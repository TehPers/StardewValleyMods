using System.ComponentModel;
using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    [JsonDescribe]
    public sealed class TreasureConfig : IModConfig
    {
        [Description("Maximum amount of treasure you can find in a single chest while fishing.")]
        public int MaxTreasureQuantity { get; set; }

        [Description(
            "Whether the treasure randomizer should be allowed to select the same loot option "
            + "multiple times. Some loot can't be repeated even if duplicate loot is enabled."
        )]
        public bool AllowDuplicateLoot { get; set; }

        [Description("Affects how fast you catch treasure.")]
        public float CatchSpeed { get; set; }

        [Description("Affects how fast the treasure bar drains when the bobber isn't on the chest.")]
        public float DrainSpeed { get; set; }

        [Description("The chances of finding treasure while fishing.")]
        public TreasureChances TreasureChances { get; init; } = new();

        [Description(
            "The chances of getting additional loot when finding treasure. This is rolled until it "
            + "fails or the max number of items have been added to the loot."
        )]
        public FishingChances AdditionalLootChances { get; init; } = new();

        // TODO: treasure quality config values for increased quality at higher streak levels and such

        public void Reset()
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

        public void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.treasure.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.treasure.{key}.desc");

            configApi.RegisterSimpleOption(
                manifest,
                Name("maxTreasureQuantity"),
                Desc("maxTreasureQuantity"),
                () => this.MaxTreasureQuantity,
                val => this.MaxTreasureQuantity = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("allowDuplicateLoot"),
                Desc("allowDuplicateLoot"),
                () => this.AllowDuplicateLoot,
                val => this.AllowDuplicateLoot = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("catchSpeed"),
                Desc("catchSpeed"),
                () => this.CatchSpeed,
                val => this.CatchSpeed = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("drainSpeed"),
                Desc("drainSpeed"),
                () => this.DrainSpeed,
                val => this.DrainSpeed = val
            );

            // Treasure chances
            configApi.RegisterLabel(manifest, Name("treasureChances"), Desc("treasureChances"));
            this.TreasureChances.RegisterOptions(configApi, manifest, translations);

            // Additional loot chances
            configApi.RegisterLabel(manifest, Name("additionalLootChances"), Desc("additionalLootChances"));
            this.AdditionalLootChances.RegisterOptions(configApi, manifest, translations);
        }
    }
}
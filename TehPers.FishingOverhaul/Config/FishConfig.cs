using System.ComponentModel;
using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    [JsonDescribe]
    public sealed class FishConfig : JsonConfigRoot, IModConfig
    {
        [Description(
            "Whether this mod affects vanilla legendary fish at all. If false, vanilla legendary "
            + "fish will be caught in the same places and with the same chances as the vanilla game."
        )]
        public bool ShouldOverrideVanillaLegendaries { get; set; }

        [Description("Whether all farm types should have fish.")]
        public bool AllowFishOnAllFarms { get; set; }

        [Description("Whether to show the fish being caught in the fishing minigame.")]
        public bool ShowFishInMinigame { get; set; }

        [Description("Affects how fast you catch fish.")]
        public float CatchSpeed { get; set; }

        [Description("Affects how fast the catch bar drains when the bobber isn't on the fish.")]
        public float DrainSpeed { get; set; }

        [Description(
            "Required streak for an increase in quality. For example, 3 means that every 3 "
            + "consecutive perfect catches increases your catch quality by 1."
        )]
        public int StreakForIncreasedQuality { get; set; }

        [Description(
            "Determines the max quality fish a non-perfect catch can get, or null for no "
            + "restrictions."
        )]
        public int? MaxNormalFishQuality { get; set; }

        [Description("The chance that you'll find a fish instead of trash.")]
        public FishingChances FishChances { get; init; } = new();

        [Description(
            "The max quality fish that can be caught. 0 = normal, 1 = silver, 2 = gold, 3 = "
            + "iridium, 4+ = beyond iridium."
        )]
        public int MaxFishQuality { get; set; }

        public void Reset()
        {
            this.ShouldOverrideVanillaLegendaries = true;
            this.AllowFishOnAllFarms = false;
            this.ShowFishInMinigame = false;
            this.CatchSpeed = 1f;
            this.DrainSpeed = 1f;
            this.StreakForIncreasedQuality = 3;
            this.MaxNormalFishQuality = null;
            this.MaxFishQuality = 3;

            // Fish chances
            this.FishChances.BaseChance = 0.5;
            this.FishChances.StreakFactor = 0.005;
            this.FishChances.FishingLevelFactor = 0.025;
            this.FishChances.DailyLuckFactor = 1;
            this.FishChances.LuckLevelFactor = 0.01;
            this.FishChances.MinChance = 0.1;
            this.FishChances.MaxChance = 0.9;
        }

        public void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.fish.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.fish.{key}.desc");

            configApi.RegisterSimpleOption(
                manifest,
                Name("shouldOverrideVanillaLegendaries"),
                Desc("shouldOverrideVanillaLegendaries"),
                () => this.ShouldOverrideVanillaLegendaries,
                val => this.ShouldOverrideVanillaLegendaries = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("allowFishOnAllFarms"),
                Desc("allowFishOnAllFarms"),
                () => this.AllowFishOnAllFarms,
                val => this.AllowFishOnAllFarms = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("showFishInMinigame"),
                Desc("showFishInMinigame"),
                () => this.ShowFishInMinigame,
                val => this.ShowFishInMinigame = val
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("catchSpeed"),
                Desc("catchSpeed"),
                () => this.CatchSpeed,
                val => this.CatchSpeed = val,
                0f,
                3f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("drainSpeed"),
                Desc("drainSpeed"),
                () => this.DrainSpeed,
                val => this.DrainSpeed = val,
                0f,
                3f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("streakForIncreasedQuality"),
                Desc("streakForIncreasedQuality"),
                () => this.StreakForIncreasedQuality,
                val => this.StreakForIncreasedQuality = val,
                0,
                100
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("maxNormalFishQuality.enabled"),
                Desc("maxNormalFishQuality.enabled"),
                () => this.MaxNormalFishQuality is not null,
                val => this.MaxNormalFishQuality = val ? 0 : null
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("maxNormalFishQuality"),
                Desc("maxNormalFishQuality"),
                () => this.MaxNormalFishQuality ?? 0,
                val =>
                {
                    if (this.MaxNormalFishQuality is not null)
                    {
                        this.MaxNormalFishQuality = val;
                    }
                },
                0,
                4
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("maxFishQuality"),
                Desc("maxFishQuality"),
                () => this.MaxFishQuality,
                val => this.MaxFishQuality = val,
                0,
                3
            );

            // Fish chances
            configApi.RegisterLabel(manifest, Name("fishChances"), Desc("fishChances"));
            this.FishChances.RegisterOptions(configApi, manifest, translations);
        }
    }
}
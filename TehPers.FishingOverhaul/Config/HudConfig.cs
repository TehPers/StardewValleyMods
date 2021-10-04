using System.ComponentModel;
using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    [JsonDescribe]
    public sealed class HudConfig : JsonConfigRoot, IModConfig
    {
        [Description(
            "Whether or not to show current streak, chance for treasure, chance for each fish, "
            + "etc. while fishing."
        )]
        public bool ShowFishingHud { get; set; }

        [Description("The X coordinate of the top left corner of the fishing HUD.")]
        public int TopLeftX { get; set; }

        [Description("The Y coordinate of the top left corner of the fishing HUD.")]
        public int TopLeftY { get; set; }

        [Description("The number of fish to show on the fishing HUD.")]
        public int MaxFishTypes { get; set; }

        public void Reset()
        {
            this.ShowFishingHud = true;
            this.TopLeftX = 0;
            this.TopLeftY = 0;
            this.MaxFishTypes = 5;
        }

        public void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.hud.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.hud.{key}.desc");

            configApi.RegisterSimpleOption(
                manifest,
                Name("showFishingHud"),
                Desc("showFishingHud"),
                () => this.ShowFishingHud,
                val => this.ShowFishingHud = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("topLeftX"),
                Desc("topLeftX"),
                () => this.TopLeftX,
                val => this.TopLeftX = val
            );
            configApi.RegisterSimpleOption(
                manifest,
                Name("topLeftY"),
                Desc("topLeftY"),
                () => this.TopLeftY,
                val => this.TopLeftY = val
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("maxFishTypes"),
                Desc("maxFishTypes"),
                () => this.MaxFishTypes,
                val => this.MaxFishTypes = val,
                0,
                20
            );
        }
    }
}
using System.ComponentModel;
using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for the fishing HUD.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
    [JsonDescribe]
    public sealed class HudConfig : IModConfig
    {
        /// <summary>
        /// Whether or not to show current streak, chance for treasure, chance for each fish, etc.
        /// while fishing.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowFishingHud { get; set; } = true;

        /// <summary>
        /// The X coordinate of the top left corner of the fishing HUD.
        /// </summary>
        [DefaultValue(0)]
        public int TopLeftX { get; set; }

        /// <summary>
        /// The Y coordinate of the top left corner of the fishing HUD.
        /// </summary>
        [DefaultValue(0)]
        public int TopLeftY { get; set; }

        /// <summary>
        /// The number of fish to show on the fishing HUD.
        /// </summary>
        [DefaultValue(5)]
        public int MaxFishTypes { get; set; } = 5;

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

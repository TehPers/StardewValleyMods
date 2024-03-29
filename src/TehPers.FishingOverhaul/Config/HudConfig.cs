﻿using System.ComponentModel;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for the fishing HUD.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
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

        /// <summary>
        /// Whether to show trash in the fishing HUD.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowTrash { get; set; }

        void IModConfig.Reset()
        {
            this.ShowFishingHud = true;
            this.TopLeftX = 0;
            this.TopLeftY = 0;
            this.MaxFishTypes = 5;
        }

        void IModConfig.RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.hud.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.hud.{key}.desc");

            configApi.AddBoolOption(
                manifest,
                () => this.ShowFishingHud,
                val => this.ShowFishingHud = val,
                () => Name("showFishingHud"),
                () => Desc("showFishingHud")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.TopLeftX,
                val => this.TopLeftX = val,
                () => Name("topLeftX"),
                () => Desc("topLeftX")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.TopLeftY,
                val => this.TopLeftY = val,
                () => Name("topLeftY"),
                () => Desc("topLeftY")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.MaxFishTypes,
                val => this.MaxFishTypes = val,
                () => Name("maxFishTypes"),
                () => Desc("maxFishTypes"),
                0,
                20
            );
            configApi.AddBoolOption(
                manifest,
                () => this.ShowTrash,
                val => this.ShowTrash = val,
                () => Name("showTrash"),
                () => Desc("showTrash")
            );
        }
    }
}

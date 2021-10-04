using System;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class GenericModConfigMenuSetup : ISetup
    {
        private readonly IModHelper helper;
        private readonly IManifest manifest;
        private readonly Func<IOptional<IGenericModConfigMenuApi>> configApiFactory;
        private readonly HudConfig hudConfig;
        private readonly ConfigManager<HudConfig> hudConfigManager;
        private readonly FishConfig fishConfig;
        private readonly ConfigManager<FishConfig> fishConfigManager;
        private readonly TreasureConfig treasureConfig;
        private readonly ConfigManager<TreasureConfig> treasureConfigManager;

        public GenericModConfigMenuSetup(
            IModHelper helper,
            IManifest manifest,
            Func<IOptional<IGenericModConfigMenuApi>> configApiFactory,
            HudConfig hudConfig,
            ConfigManager<HudConfig> hudConfigManager,
            FishConfig fishConfig,
            ConfigManager<FishConfig> fishConfigManager,
            TreasureConfig treasureConfig,
            ConfigManager<TreasureConfig> treasureConfigManager
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.configApiFactory = configApiFactory ?? throw new ArgumentNullException(nameof(configApiFactory));
            this.hudConfig = hudConfig ?? throw new ArgumentNullException(nameof(hudConfig));
            this.hudConfigManager = hudConfigManager ?? throw new ArgumentNullException(nameof(hudConfigManager));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.fishConfigManager = fishConfigManager ?? throw new ArgumentNullException(nameof(fishConfigManager));
            this.treasureConfig = treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.treasureConfigManager =
                treasureConfigManager ?? throw new ArgumentNullException(nameof(treasureConfigManager));
        }

        public void Setup()
        {
            this.helper.Events.GameLoop.GameLaunched += (_, _) =>
            {
                if (!this.configApiFactory().TryGetValue(out var configApi))
                {
                    return;
                }

                Translation Name(string key) => this.helper.Translation.Get($"text.config.{key}.name");
                Translation Desc(string key) => this.helper.Translation.Get($"text.config.{key}.desc");

                configApi.RegisterModConfig(
                    this.manifest,
                    () =>
                    {
                        this.hudConfig.Reset();
                        this.fishConfig.Reset();
                        this.treasureConfig.Reset();
                    },
                    () =>
                    {
                        this.hudConfigManager.Save(this.hudConfig);
                        this.fishConfigManager.Save(this.fishConfig);
                        this.treasureConfigManager.Save(this.treasureConfig);
                    }
                );

                configApi.SetDefaultIngameOptinValue(this.manifest, true);
                configApi.RegisterPageLabel(this.manifest, Name("hud"), Desc("hud"), Name("hud"));
                configApi.RegisterPageLabel(this.manifest, Name("fish"), Desc("fish"), Name("fish"));
                configApi.RegisterPageLabel(this.manifest, Name("treasure"), Desc("treasure"), Name("treasure"));

                // HUD config settings
                configApi.StartNewPage(this.manifest, Name("hud"));
                this.hudConfig.RegisterOptions(configApi, this.manifest, this.helper.Translation);

                // Fishing config settings
                configApi.StartNewPage(this.manifest, Name("fish"));
                this.fishConfig.RegisterOptions(configApi, this.manifest, this.helper.Translation);

                // Treasure config settings
                configApi.StartNewPage(this.manifest, Name("treasure"));
                this.treasureConfig.RegisterOptions(configApi, this.manifest, this.helper.Translation);
            };
        }
    }
}
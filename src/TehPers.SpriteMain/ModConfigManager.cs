using System;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Json;
using TehPers.Core.Api.Setup;
using TehPers.SpriteMain.Integrations.GenericModConfigMenu;
using TehPers.SpriteMain.Patches;
using TehPers.SpriteMain.Scalers;

namespace TehPers.SpriteMain
{
    internal class ModConfigManager : ISetup, IDisposable
    {
        private readonly IManifest manifest;
        private readonly IJsonProvider jsonProvider;
        private readonly SpriteBatchPatcher patcher;
        private readonly IOptional<IGenericModConfigMenuApi> gmcmApi;
        private readonly string path;

        public ModConfig CurrentConfig { get; private set; }

        public ModConfigManager(
            IManifest manifest,
            IJsonProvider jsonProvider,
            SpriteBatchPatcher patcher,
            IOptional<IGenericModConfigMenuApi> gmcmApi,
            string path
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.jsonProvider = jsonProvider;
            this.patcher = patcher ?? throw new ArgumentNullException(nameof(patcher));
            this.gmcmApi = gmcmApi ?? throw new ArgumentNullException(nameof(gmcmApi));
            this.path = path ?? throw new ArgumentNullException(nameof(path));

            this.CurrentConfig = new();
        }

        public void Setup()
        {
            this.CurrentConfig = this.jsonProvider.ReadOrCreate<ModConfig>(this.path);
            this.EnableScaler(this.CurrentConfig.Scaler);

            if (!this.gmcmApi.TryGetValue(out var gmcmApi))
            {
                return;
            }

            gmcmApi.Register(
                this.manifest,
                () => this.CurrentConfig = new(),
                () => this.jsonProvider.WriteJson(this.CurrentConfig, this.path)
            );

            gmcmApi.AddTextOption(
                this.manifest,
                () => this.CurrentConfig.Scaler switch
                {
                    ScalerName.Scale2X => "Scale2X",
                    ScalerName.Scale3X => "Scale3X",
                    _ => "None",
                },
                value =>
                {
                    ScalerName? scalerName = value switch
                    {
                        "Scale2X" => ScalerName.Scale2X,
                        "Scale3X" => ScalerName.Scale3X,
                        _ => null
                    };
                    this.EnableScaler(scalerName);
                    this.CurrentConfig.Scaler = scalerName;
                },
                () => "Scaler",
                () => "The active scaler to use",
                new[] { "None", "Scale2X", "Scale3X" }
            );
        }

        public void Dispose()
        {
            if (!this.gmcmApi.TryGetValue(out var gmcmApi))
            {
                return;
            }

            gmcmApi.Unregister(this.manifest);
        }

        private void EnableScaler(ScalerName? scalerName)
        {
            this.patcher.SetScaler(
                scalerName switch
                {
                    ScalerName.Scale2X => new Scale2XScaler(),
                    ScalerName.Scale3X => new Scale3XScaler(),
                    _ => null
                }
            );
        }
    }
}
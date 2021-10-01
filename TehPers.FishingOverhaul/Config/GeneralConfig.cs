using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    [JsonDescribe]
    public sealed class GeneralConfig : IModConfig
    {
        public void Reset()
        {
        }

        public void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.general.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.general.{key}.desc");
        }
    }
}
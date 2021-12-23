using StardewModdingAPI;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    internal interface IModConfig
    {
        void Reset();

        void RegisterOptions(IGenericModConfigMenuApi configApi, IManifest manifest, ITranslationHelper translations);
    }
}
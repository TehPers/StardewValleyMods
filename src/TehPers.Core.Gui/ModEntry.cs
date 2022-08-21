using StardewModdingAPI;
using TehPers.Core.Gui.Api.Extensions;

namespace TehPers.Core.Gui;

public class ModEntry : Mod
{
    private CoreGuiApi? api;

    public override void Entry(IModHelper helper)
    {
        // Setup API
        this.api = new();
        ModInitializer.InitializeGuiApi(this.api);
    }

    public override object? GetApi()
    {
        return this.api;
    }
}

using StardewModdingAPI;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui;

internal class SimpleManagedMenu : ManagedMenu
{
    private readonly IGuiComponent root;

    public SimpleManagedMenu(IGuiComponent root, IModHelper helper)
        : base(helper)
    {
        this.root = root;
    }

    protected override IGuiComponent CreateRoot()
    {
        return this.root;
    }
}

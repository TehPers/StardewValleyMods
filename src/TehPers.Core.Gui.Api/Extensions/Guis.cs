using StardewModdingAPI;
using StardewValley.Menus;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Api.Extensions;

public static class Guis
{
    public static IClickableMenu ToMenu<TMessage>(this IGui<TMessage> gui, IGuiBuilder ui, IModHelper helper)
    {
        return new ManagedGuiMenu<TMessage>(gui, ui, helper);
    }
}

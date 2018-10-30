using StardewValley.Menus;
using TehPers.Core.Gui.Components;

namespace TehPers.Core.Gui {
    public interface IGuiApi {
        IClickableMenu ConvertMenu(IGuiComponent menu);
    }
}
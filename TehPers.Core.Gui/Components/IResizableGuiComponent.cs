using TehPers.Core.Gui.Units.Base;
using TehPers.Core.Gui.Units.SDV;

namespace TehPers.Core.Gui.Components {
    public interface IResizableGuiComponent : IGuiComponent {
        /// <inheritdoc cref="IGuiComponent.Location"/>
        new ResponsiveVector2<GuiInfo> Location { get; set; }

        /// <inheritdoc cref="IGuiComponent.Size"/>
        new ResponsiveVector2<GuiInfo> Size { get; set; }
    }
}

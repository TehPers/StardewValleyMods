using System.Collections.Generic;
using TehPers.Core.Gui.Units.Base;
using TehPers.Core.Gui.Units.SDV;

namespace TehPers.Core.Gui.Components {
    public abstract class ResizableGuiComponent : GuiComponent, IResizableGuiComponent {
        protected ResizableGuiComponent() { }
        protected ResizableGuiComponent(IGuiComponent parent) : base(parent) { }
        protected ResizableGuiComponent(IEnumerable<IGuiComponent> children) : base(children) { }
        protected ResizableGuiComponent(IGuiComponent parent, IEnumerable<IGuiComponent> children) : base(parent, children) { }

        /// <inheritdoc cref="GuiComponent.Location"/>
        public new virtual ResponsiveVector2<GuiInfo> Location { get => base.Location; set => base.Location = value; }

        /// <inheritdoc cref="GuiComponent.Size"/>
        public new virtual ResponsiveVector2<GuiInfo> Size { get => base.Size; set => base.Size = value; }
    }
}

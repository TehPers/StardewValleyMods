using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using TehPers.Core.Api.Gui.States;

namespace TehPers.Core.Api.Gui.Components
{
    internal record Checkbox(CheckboxState State) : ComponentWrapper
    {
        public override IGuiComponent Inner => GuiComponent.Texture(
            Game1.mouseCursors,
            sourceRectangle: this.State.Checked
                ? OptionsCheckbox.sourceRectChecked
                : OptionsCheckbox.sourceRectUnchecked,
            minScale: GuiSize.One,
            maxScale: PartialGuiSize.One
        );

        public override void Handle(GuiEvent e, Rectangle bounds)
        {
            if (e.Clicked(bounds, ClickType.Left))
            {
                this.State.Checked ^= true;
            }

            base.Handle(e, bounds);
        }
    }
}

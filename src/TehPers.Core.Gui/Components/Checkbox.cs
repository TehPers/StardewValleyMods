using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="ICheckbox"/>
internal record Checkbox(IGuiBuilder Builder, ICheckbox.IState State) : BaseGuiComponent(Builder),
    ICheckbox
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.CreateInner().GetConstraints();
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        if (e.Clicked(bounds, ClickType.Left))
        {
            this.State.Checked ^= true;
        }

        this.CreateInner().Handle(e, bounds);
    }

    private IGuiComponent CreateInner()
    {
        return this.GuiBuilder.Texture(Game1.mouseCursors)
            .WithSourceRectangle(
                this.State.Checked
                    ? OptionsCheckbox.sourceRectChecked
                    : OptionsCheckbox.sourceRectUnchecked
            )
            .WithMinScale(GuiSize.One)
            .WithMaxScale(PartialGuiSize.One);
    }

    /// <inheritdoc />
    public ICheckbox WithState(ICheckbox.IState state)
    {
        return this with {State = state};
    }
}

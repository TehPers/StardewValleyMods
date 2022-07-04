using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A component that executes an action when clicked.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    /// <param name="OnClick">The action to perform when the button is clicked.</param>
    internal record ClickHandler(IGuiComponent Inner, Action<ClickType> OnClick) : WrapperComponent(Inner)
    {
        /// <inheritdoc />
        public override void Handle(GuiEvent e, Rectangle bounds)
        {
            base.Handle(e, bounds);
            if (e.ClickType(bounds) is { } clickType)
            {
                this.OnClick(clickType);
            }
        }
    }
}

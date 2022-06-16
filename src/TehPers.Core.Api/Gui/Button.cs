using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A button that can be clicked.
    /// </summary>
    /// <param name="Label">The button label component.</param>
    /// <param name="OnClick">The action to perform when the button is clicked.</param>
    public record Button(IGuiComponent Label, Action<ClickType> OnClick) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Label.GetConstraints();
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            this.Label.Handle(e, bounds);
            if (e.ClickType(bounds) is { } clickType)
            {
                this.OnClick(clickType);
            }
        }
    }
}

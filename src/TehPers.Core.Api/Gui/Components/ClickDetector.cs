using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// A component that executes an action when clicked.
    /// </summary>
    internal record ClickDetector : ComponentWrapper
    {
        /// <summary>
        /// The inner component.
        /// </summary>
        public override IGuiComponent Inner { get; }

        /// <summary>
        /// The action to perform when clicked.
        /// </summary>
        public Action<ClickType> OnClick { get; init; }


        /// <summary>
        /// A component that executes an action when clicked.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        /// <param name="onClick">The action to perform when clicked.</param>
        public ClickDetector(IGuiComponent inner, Action<ClickType> onClick)
        {
            this.Inner = inner;
            this.OnClick = onClick;
        }

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

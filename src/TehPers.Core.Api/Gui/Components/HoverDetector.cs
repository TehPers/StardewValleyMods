using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// A component that executes an action when hovered.
    /// </summary>
    internal record HoverDetector : ComponentWrapper
    {
        /// <summary>
        /// The inner component.
        /// </summary>
        public override IGuiComponent Inner { get; }

        /// <summary>
        /// The action to perform when hovered.
        /// </summary>
        public Action OnHover { get; init; }


        /// <summary>
        /// A component that executes an action when clicked.
        /// </summary>
        /// <param name="inner">The inner component.</param>
        /// <param name="onHover">The action to perform when hovered.</param>
        public HoverDetector(IGuiComponent inner, Action onHover)
        {
            this.Inner = inner;
            this.OnHover = onHover;
        }

        /// <inheritdoc />
        public override void Handle(GuiEvent e, Rectangle bounds)
        {
            base.Handle(e, bounds);
            if (e is GuiEvent.Hover(var position) && bounds.Contains(position))
            {
                this.OnHover();
            }
        }
    }
}

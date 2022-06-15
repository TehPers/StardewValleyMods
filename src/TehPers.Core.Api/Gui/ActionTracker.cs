#if FALSE
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A component that tracks various common events.
    /// </summary>
    /// <param name="Inner">The inner component.</param> 
    public record ActionTracker(IGuiComponent Inner) : BaseGuiComponent
    {
        /// <summary>
        /// An action or transformation to apply when an update tick is received. A non-null result
        /// will replace this component with the resulting component.
        /// </summary>
        public Func<ActionTracker, IGuiComponent?>? Updated { get; init; }

        /// <summary>
        /// An action or transformation to apply when this component is hovered. A non-null result
        /// will replace this component with the resulting component.
        /// </summary>
        // TODO
        public Func<ActionTracker, IGuiComponent?>? Hovering { get; init; }

        /// <summary>
        /// An action or transformation to apply when this component is no longer hovered. A
        /// non-null result will replace this component with the resulting component.
        /// </summary>
        // TODO
        public Func<ActionTracker, IGuiComponent?>? HoverLost { get; init; }

        /// <summary>
        /// An action or transformation to apply when this component is clicked. A non-null result
        /// will replace this component with the resulting component.
        /// </summary>
        public Func<ActionTracker, ClickType, IGuiComponent?>? ClickReceived { get; init; }

        /// <inheritdoc />
        public override GuiConstraints Constraints => this.Inner.Constraints;

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.Inner.CalculateLayouts(bounds, layouts);
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            // TODO
            var changed = false;
            var result = this;
            if (this.Inner.Update(e, componentBounds, out var newInner))
            {
                changed = true;
                result = result with {Inner = newInner};
            }

            newComponent = this.Updated?.Invoke(this);

            // Need to get the bounds
            if (!componentBounds.TryGetValue(this, out var bounds))
            {
                return newComponent is not null;
            }

            // Click handling
            if (newComponent is null
                && this.ClickReceived is { } onClick
                && e is GuiEvent.ReceiveClick(var position, var clickType)
                && bounds.Contains(position)
                && onClick(this, clickType) is { } newButton)
            {
                newComponent = newButton;
            }

            return newComponent is not null;
        }
    }
}
#endif

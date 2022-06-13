using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A button that can be clicked.
    /// </summary>
    /// <param name="Label">The button label component.</param>
    public record Button(IGuiComponent Label) : BaseGuiComponent
    {
        /// <summary>
        /// An action or transformation to apply when this button is clicked.
        /// </summary>
        public Func<Button, ClickType, Button?>? OnClick { get; init; }

        private readonly IGuiComponent inner = Label.WithBackground(new EmptySpace());

        /// <inheritdoc />
        public override GuiConstraints Constraints => this.inner.Constraints;

        /// <inheritdoc />
        public override void Draw(SpriteBatch batch, Rectangle bounds)
        {
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            if (componentBounds.TryGetValue(this, out var bounds))
            {
                // Click handling
                if (this.OnClick is { } onClick
                    && e is GuiEvent.ReceiveClick(var position, var clickType)
                    && bounds.Contains(position)
                    && onClick(this, clickType) is { } newButton)
                {
                    newComponent = newButton;
                    return true;
                }
            }

            newComponent = default;
            return false;
        }
    }
}

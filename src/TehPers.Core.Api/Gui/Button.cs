using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A button that can be clicked.
    /// </summary>
    /// <param name="Label">The button label component.</param>
    public record Button(IGuiComponent<Unit> Label) : IGuiComponent<Button.Response>
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return this.Label.GetConstraints();
        }

        /// <inheritdoc />
        public Response Handle(GuiEvent e, Rectangle bounds)
        {
            this.Label.Handle(e, bounds);
            return new(e.ClickType(bounds));
        }

        /// <summary>
        /// A response from a <see cref="Button"/> component.
        /// </summary>
        public class Response
        {
            /// <summary>
            /// The type of click that the button received, if any.
            /// </summary>
            public ClickType? ClickType { get; }

            /// <summary>
            /// Whether the button was clicked.
            /// </summary>
            public bool Clicked => this.ClickType is not null;

            internal Response(ClickType? clickType)
            {
                this.ClickType = clickType;
            }
        }
    }
}

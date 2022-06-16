using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Empty space in a GUI.
    /// </summary>
    public record EmptySpace : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            return new();
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
        }
    }
}

using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Shrinks a GUI component to its minimum size.
    /// </summary>
    /// <param name="Inner">The inner component.</param>
    public record Shrink(IGuiComponent Inner) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var innerConstraints = this.Inner.GetConstraints();
            return innerConstraints with
            {
                MaxSize = new(innerConstraints.MinSize),
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            this.Inner.Handle(e, bounds);
        }
    }
}

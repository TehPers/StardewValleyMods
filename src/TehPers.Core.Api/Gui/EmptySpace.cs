using Microsoft.Xna.Framework;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Empty space in a GUI.
    /// </summary>
    public record EmptySpace : BaseGuiComponent
    {
        /// <inheritdoc />
        public override GuiConstraints Constraints { get; } = new();

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            newComponent = default;
            return false;
        }
    }
}

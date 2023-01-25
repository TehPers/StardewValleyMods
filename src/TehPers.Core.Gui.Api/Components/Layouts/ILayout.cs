using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Gui.Api.Components.Layouts;

/// <summary>
/// A GUI layout.
/// </summary>
/// <typeparam name="TLayout">The interface for the concrete layout type.</typeparam>
public interface ILayout<out TLayout> : IGuiComponent
{
    /// <summary>
    /// Sets the components that are a part of this layout.
    /// </summary>
    /// <param name="components">The new components that are a part of this layout.</param>
    /// <returns>The resulting component.</returns>
    TLayout WithComponents(IEnumerable<IGuiComponent> components);

    /// <summary>
    /// Sets the components that are a part of this layout.
    /// </summary>
    /// <param name="components">The new components that are a part of this layout.</param>
    /// <returns>The resulting component.</returns>
    TLayout WithComponents(params IGuiComponent[] components)
    {
        return this.WithComponents(components.AsEnumerable());
    }
}

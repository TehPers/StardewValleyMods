using System;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component that executes an action when hovered.
/// </summary>
public interface IHoverDetector : IGuiComponent, IWithInner<IHoverDetector>
{
    /// <summary>
    /// Sets the action to execute when this component is hovered.
    /// </summary>
    /// <param name="action">The new action to execute.</param>
    /// <returns>The resulting component.</returns>
    IHoverDetector WithAction(Action action);
}

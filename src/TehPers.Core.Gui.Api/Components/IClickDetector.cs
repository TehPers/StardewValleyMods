using System;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component which detects a click.
/// </summary>
public interface IClickDetector : IGuiComponent, IWithInner<IClickDetector>
{
    /// <summary>
    /// Sets the action to execute when this component is clicked.
    /// </summary>
    /// <param name="action">The new action to execute.</param>
    /// <returns>The resulting component.</returns>
    IClickDetector WithAction(Action<ClickType> action);
}

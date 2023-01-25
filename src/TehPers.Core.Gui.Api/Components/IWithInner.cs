namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component which contains an inner component.
/// </summary>
/// <typeparam name="TComponent">The component type.</typeparam>
public interface IWithInner<out TComponent>
{
    /// <summary>
    /// Sets the inner component.
    /// </summary>
    /// <param name="inner">The new inner component.</param>
    /// <returns>The resulting component.</returns>
    TComponent WithInner(IGuiComponent inner);
}

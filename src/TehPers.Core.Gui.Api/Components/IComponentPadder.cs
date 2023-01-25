namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// Adds padding to a component.
/// </summary>
public interface IComponentPadder : IGuiComponent, IWithInner<IComponentPadder>
{
    /// <summary>
    /// Sets the left padding of this component.
    /// </summary>
    /// <param name="padding">The new padding.</param>
    /// <returns>The resulting component.</returns>
    IComponentPadder WithLeft(float padding);

    /// <summary>
    /// Sets the right padding of this component.
    /// </summary>
    /// <param name="padding">The new padding.</param>
    /// <returns>The resulting component.</returns>
    IComponentPadder WithRight(float padding);

    /// <summary>
    /// Sets the top padding of this component.
    /// </summary>
    /// <param name="padding">The new padding.</param>
    /// <returns>The resulting component.</returns>
    IComponentPadder WithTop(float padding);

    /// <summary>
    /// Sets the bottom padding of this component.
    /// </summary>
    /// <param name="padding">The new padding.</param>
    /// <returns>The resulting component.</returns>
    IComponentPadder WithBottom(float padding);
}

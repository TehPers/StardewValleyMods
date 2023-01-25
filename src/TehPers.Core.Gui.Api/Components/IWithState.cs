namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component which contains state.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TComponent">The component type.</typeparam>
public interface IWithState<in TState, out TComponent>
{
    /// <summary>
    /// Sets the state of this component.
    /// </summary>
    /// <param name="state">The new state of this component.</param>
    /// <returns>The resulting component.</returns>
    TComponent WithState(TState state);
}

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component with a configurable layer depth.
/// </summary>
/// <typeparam name="TComponent">The component type.</typeparam>
public interface IWithLayerDepth<out TComponent>
{
    /// <summary>
    /// Sets the layer depth this component is rendered at.
    /// </summary>
    /// <param name="layerDepth">The new layer depth to render the component at.</param>
    /// <returns>The resulting component.</returns>
    TComponent WithLayerDepth(float layerDepth);
}

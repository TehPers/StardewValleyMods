namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A component with a background.
/// </summary>
public interface IBackground : IGuiComponent, IWithInner<IBackground>
{
    /// <summary>
    /// Sets the background component.
    /// </summary>
    /// <param name="background">The new background component.</param>
    /// <returns>The resulting component.</returns>
    IBackground WithBackground(IGuiComponent background);
}

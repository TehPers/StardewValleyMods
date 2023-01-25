namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// Clips this component, removing its minimum size constraint. This constrains its rendering
/// area and mouse inputs if it is shrunk.
/// </summary>
public interface IClipper : IGuiComponent, IWithInner<IClipper>
{
}

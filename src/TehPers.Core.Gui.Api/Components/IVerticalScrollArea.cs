namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// A vertically scrollable view that clips its inner component.
/// </summary>
public interface IVerticalScrollArea : IGuiComponent, IWithInner<IVerticalScrollArea>,
    IWithState<IVerticalScrollbar.IState, IVerticalScrollArea>
{
}

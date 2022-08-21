using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IHoverDetector"/>
internal record HoverDetector
    (IGuiBuilder Builder, IGuiComponent Inner, Action Action) : ComponentWrapper(Builder, Inner),
        IHoverDetector
{
    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        base.Handle(e, bounds);
        if (e.IsHover(out var position) && bounds.Contains(position))
        {
            this.Action();
        }
    }

    public IHoverDetector WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    public IHoverDetector WithAction(Action action)
    {
        return this with {Action = action};
    }
}

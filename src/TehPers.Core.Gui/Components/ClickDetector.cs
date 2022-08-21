using Microsoft.Xna.Framework;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IClickDetector"/>
internal record ClickDetector(
    IGuiBuilder Builder,
    IGuiComponent Inner,
    Action<ClickType> Action
) : ComponentWrapper(Builder, Inner), IClickDetector
{
    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        base.Handle(e, bounds);
        if (e.ClickType(bounds) is { } clickType)
        {
            this.Action(clickType);
        }
    }

    /// <inheritdoc />
    public IClickDetector WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    /// <inheritdoc />
    public IClickDetector WithAction(Action<ClickType> action)
    {
        return this with {Action = action};
    }
}

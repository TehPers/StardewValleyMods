using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="ISimpleComponent"/>
internal record SimpleComponent(
    IGuiBuilder Builder,
    IGuiConstraints Constraints,
    Action<SpriteBatch, Rectangle> Draw
) : BaseGuiComponent(Builder), ISimpleComponent
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.Constraints;
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        e.Draw(batch => this.Draw(batch, bounds));
    }

    /// <inheritdoc />
    public ISimpleComponent WithConstraints(IGuiConstraints constraints)
    {
        return this with {Constraints = constraints};
    }

    /// <inheritdoc />
    public ISimpleComponent WithDrawAction(Action<SpriteBatch, Rectangle> draw)
    {
        return this with {Draw = draw};
    }
}

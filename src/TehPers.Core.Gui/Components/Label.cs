using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="ILabel" />
internal record Label(IGuiBuilder Builder, string Text) : BaseGuiComponent(Builder), ILabel
{
    public SpriteFont Font { get; init; } = Game1.smallFont;
    public Color Color { get; init; } = Game1.textColor;
    public Vector2 Scale { get; init; } = Vector2.One;
    public SpriteEffects SpriteEffects { get; init; } = SpriteEffects.None;
    public float LayerDepth { get; init; }

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var size = this.Font.MeasureString(this.Text);
        return new GuiConstraints(new GuiSize(size), new PartialGuiSize(size));
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        e.Draw(
            batch => batch.DrawString(
                this.Font,
                this.Text,
                new Vector2(bounds.X, bounds.Y),
                this.Color,
                0,
                Vector2.Zero,
                this.Scale,
                this.SpriteEffects,
                this.LayerDepth
            )
        );
    }

    /// <inheritdoc />
    public ILabel WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }

    /// <inheritdoc />
    public ILabel WithText(string text)
    {
        return this with {Text = text};
    }

    /// <inheritdoc />
    public ILabel WithFont(SpriteFont font)
    {
        return this with {Font = font};
    }

    /// <inheritdoc />
    public ILabel WithColor(Color color)
    {
        return this with {Color = color};
    }

    /// <inheritdoc />
    public ILabel WithScale(Vector2 scale)
    {
        return this with {Scale = scale};
    }

    /// <inheritdoc />
    public ILabel WithSpriteEffects(SpriteEffects spriteEffects)
    {
        return this with {SpriteEffects = spriteEffects};
    }
}

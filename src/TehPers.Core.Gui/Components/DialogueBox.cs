using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IDialogueBox"/>
internal record DialogueBox(IGuiBuilder Builder) : BaseGuiComponent(Builder), IDialogueBox
{
    public IDialogueBox.Portrait Speaker { get; init; }
    public bool DrawOnlyBox { get; init; } = true;
    public string? Message { get; init; }

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return new GuiConstraints(MinSize: new GuiSize(64 * 3, 64 * 3));
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        e.Draw(
            batch =>
            {
                var prevBatch = Game1.spriteBatch;
                Game1.spriteBatch = batch;
                Game1.drawDialogueBox(
                    bounds.X,
                    bounds.Y,
                    bounds.Width,
                    bounds.Height,
                    this.Speaker is not IDialogueBox.Portrait.None,
                    this.DrawOnlyBox,
                    this.Message,
                    this.Speaker is IDialogueBox.Portrait.ObjectPortrait
                );
                Game1.spriteBatch = prevBatch;
            }
        );
    }

    /// <inheritdoc />
    public IDialogueBox WithSpeakerPortrait(IDialogueBox.Portrait portrait)
    {
        return this with {Speaker = portrait};
    }

    /// <inheritdoc />
    public IDialogueBox WithDrawOnlyBox(bool drawOnlyBox)
    {
        return this with {DrawOnlyBox = drawOnlyBox};
    }

    /// <inheritdoc />
    public IDialogueBox WithMessage(string? message)
    {
        return this with {Message = message};
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="ITextBox"/>
internal record TextBox
    (IGuiBuilder Builder, ITextInput.IState State, IInputHelper InputHelper) : BaseGuiComponent(
        Builder
    ), ITextBox
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.CreateInner().GetConstraints();
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        this.CreateInner().Handle(e, bounds);
    }

    private IGuiComponent CreateInner()
    {
        return this.GuiBuilder.TextInput(this.State, this.InputHelper)
            .WithHighlightedTextBackgroundColor(new(Color.DeepSkyBlue, 0.5f))
            .WithCursorColor(new(Color.Black, 0.75f))
            .WithPadding(16, 6, 6, 8)
            .WithBackground(
                this.GuiBuilder.TextureBox(
                        Game1.content.Load<Texture2D>(@"LooseSprites\textBox"),
                        new(0, 0, 16, 16),
                        new(16, 0, 160, 16),
                        new(176, 0, 16, 16),
                        new(0, 16, 16, 16),
                        new(16, 16, 160, 16),
                        new(176, 16, 16, 16),
                        new(0, 32, 16, 16),
                        new(16, 32, 160, 16),
                        new(176, 32, 16, 16)
                    )
                    .WithMinScale(GuiSize.One)
            );
    }

    /// <inheritdoc />
    public ITextBox WithState(ITextInput.IState state)
    {
        return this with {State = state};
    }

    public ITextBox WithInputHelper(IInputHelper inputHelper)
    {
        return this with {InputHelper = inputHelper};
    }
}

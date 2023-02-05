using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;
using TehPers.Core.Gui.Api.Guis;
using TehPers.Core.Gui.Components;
using TehPers.Core.Gui.Components.Layouts;
using Background = TehPers.Core.Gui.Components.Background;
using DialogueBox = TehPers.Core.Gui.Components.DialogueBox;
using TextBox = TehPers.Core.Gui.Components.TextBox;

namespace TehPers.Core.Gui;

/// <inheritdoc />
internal class DefaultComponentProvider : IDefaultComponentProvider
{
    private readonly IGuiBuilder builder;

    public DefaultComponentProvider(IGuiBuilder builder)
    {
        this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <inheritdoc />
    public IAligner Aligner(
        IGuiComponent inner,
        HorizontalAlignment? horizontal,
        VerticalAlignment? vertical
    )
    {
        return new Aligner(this.builder, inner, horizontal, vertical);
    }

    /// <inheritdoc />
    public IBackground Background(IGuiComponent inner, IGuiComponent background)
    {
        return new Background(this.builder, inner, background);
    }

    /// <inheritdoc />
    public ICheckbox Checkbox(ICheckbox.IState state)
    {
        return new Checkbox(this.builder, state);
    }

    /// <inheritdoc />
    public IClickDetector ClickDetector(IGuiComponent inner, Action<ClickType> action)
    {
        return new ClickDetector(this.builder, inner, action);
    }

    /// <inheritdoc />
    public IClipper Clipper(IGuiComponent inner)
    {
        return new Clipper(this.builder, inner);
    }

    /// <inheritdoc />
    public IDialogueBox DialogueBox()
    {
        return new DialogueBox(this.builder);
    }

    /// <inheritdoc />
    public IDropdown<T> Dropdown<T>(IDropdown<T>.IState state)
    {
        return new Dropdown<T>(this.builder, state);
    }

    /// <inheritdoc />
    public IComponentPadder Padder(
        IGuiComponent inner,
        float left,
        float right,
        float top,
        float bottom
    )
    {
        return new ComponentPadder(this.builder, inner, left, right, top, bottom);
    }

    /// <inheritdoc />
    public IConstrainer Constrainer(IGuiComponent inner)
    {
        return new Constrainer(this.builder, inner);
    }

    /// <inheritdoc />
    public IGuiComponent EmptySpace()
    {
        return new EmptySpace(this.builder);
    }

    /// <inheritdoc />
    public IHoverDetector HoverDetector(IGuiComponent inner, Action action)
    {
        return new HoverDetector(this.builder, inner, action);
    }

    /// <inheritdoc />
    public IItemView ItemView(Item item)
    {
        return new ItemView(this.builder, item);
    }

    /// <inheritdoc />
    public ILabel Label(string text)
    {
        return new Label(this.builder, text);
    }

    /// <inheritdoc />
    public IShrinker Shrinker(IGuiComponent inner)
    {
        return new Shrinker(this.builder, inner);
    }

    /// <inheritdoc />
    public ISimpleComponent SimpleComponent(
        IGuiConstraints constraints,
        Action<SpriteBatch, Rectangle> draw
    )
    {
        return new SimpleComponent(this.builder, constraints, draw);
    }

    /// <inheritdoc />
    public IStretchedTexture StretchedTexture(Texture2D texture)
    {
        return new StretchedTexture(this.builder, texture);
    }

    /// <inheritdoc />
    public ITextBox TextBox(ITextInput.IState state, IInputHelper inputHelper)
    {
        return new TextBox(this.builder, state, inputHelper);
    }

    /// <inheritdoc />
    public ITextInput TextInput(ITextInput.IState state, IInputHelper inputHelper)
    {
        return new TextInput(this.builder, state, inputHelper);
    }

    /// <inheritdoc />
    public ITextureBox TextureBox(
        Texture2D texture,
        Rectangle? topLeft,
        Rectangle? topCenter,
        Rectangle? topRight,
        Rectangle? centerLeft,
        Rectangle? center,
        Rectangle? centerRight,
        Rectangle? bottomLeft,
        Rectangle? bottomCenter,
        Rectangle? bottomRight
    )
    {
        return new TextureBox(
            this.builder,
            texture,
            topLeft,
            topCenter,
            topRight,
            centerLeft,
            center,
            centerRight,
            bottomLeft,
            bottomCenter,
            bottomRight
        );
    }

    /// <inheritdoc />
    public IVerticalScrollArea VerticalScrollArea(
        IGuiComponent inner,
        IVerticalScrollbar.IState state
    )
    {
        return new VerticalScrollArea(this.builder, inner, state);
    }

    /// <inheritdoc />
    public IHorizontalLayout HorizontalLayout(IEnumerable<IGuiComponent> components)
    {
        return new HorizontalLayout(this.builder, components.ToImmutableArray());
    }

    /// <inheritdoc />
    public IVerticalLayout VerticalLayout(IEnumerable<IGuiComponent> components)
    {
        return new VerticalLayout(this.builder, components.ToImmutableArray());
    }

    /// <inheritdoc />
    public IClickableMenu ComponentMenu(IGuiComponent inner, IModHelper helper)
    {
        return new SimpleManagedMenu(inner, helper);
    }
}

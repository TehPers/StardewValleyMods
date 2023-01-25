using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using TehPers.Core.Gui.Api.Components.Layouts;

namespace TehPers.Core.Gui.Api.Components;

/// <summary>
/// Creates the different types of components which are supported by default.
/// </summary>
public interface IDefaultComponentProvider
{
    /// <summary>
    /// Creates a new component aligner.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="horizontal">The horizontal alignment.</param>
    /// <param name="vertical">The vertical alignment.</param>
    /// <returns>The horizontal aligner.</returns>
    IAligner Aligner(
        IGuiComponent inner,
        HorizontalAlignment? horizontal,
        VerticalAlignment? vertical
    );

    /// <summary>
    /// Creates a new component with a background.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="background">The background component.</param>
    /// <returns></returns>
    IBackground Background(IGuiComponent inner, IGuiComponent background);

    /// <summary>
    /// Creates a new checkbox.
    /// </summary>
    /// <param name="state">The state of the checkbox.</param>
    /// <returns>The checkbox.</returns>
    ICheckbox Checkbox(ICheckbox.IState state);

    /// <summary>
    /// Creates a new click detector.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="action">The action to execute on click.</param>
    /// <returns>The click detector.</returns>
    IClickDetector ClickDetector(IGuiComponent inner, Action<ClickType> action);

    /// <summary>
    /// Clips the inner component so that it can be shrunk to any size.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <returns>The clipper.</returns>
    IClipper Clipper(IGuiComponent inner);

    /// <summary>
    /// Creates a new dialogue box.
    /// </summary>
    /// <returns>The dialogue box.</returns>
    IDialogueBox DialogueBox();

    /// <summary>
    /// Creates a new dropdown choice selector.
    /// </summary>
    /// <typeparam name="T">The type of item the dropdown holds.</typeparam>
    /// <param name="state">The state of the component.</param>
    /// <returns>The dropdown.</returns>
    IDropdown<T> Dropdown<T>(IDropdown<T>.IState state);

    /// <summary>
    /// Adds padding to the inner component.
    /// </summary>
    /// <param name="inner">The component to add padding to.</param>
    /// <param name="left">The amount of padding on the left.</param>
    /// <param name="right">The amount of padding on the right.</param>
    /// <param name="top">The amount of padding on the top.</param>
    /// <param name="bottom">The amount of padding on the bottom.</param>
    /// <returns>The padder.</returns>
    IComponentPadder Padder(IGuiComponent inner, float left, float right, float top, float bottom);

    /// <summary>
    /// Adds constraints to the inner component.
    /// </summary>
    /// <param name="inner">The component to add constraints to.</param>
    /// <returns>The constrainer.</returns>
    IConstrainer Constrainer(IGuiComponent inner);

    /// <summary>
    /// Creates an empty component that can grow or shrink to any size.
    /// </summary>
    /// <returns>The empty space.</returns>
    IGuiComponent EmptySpace();

    /// <summary>
    /// Creates a component that executes an action when hovered.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The hover detector.</returns>
    IHoverDetector HoverDetector(IGuiComponent inner, Action action);

    /// <summary>
    /// Creates a component which renders an item.
    /// </summary>
    /// <param name="item">The item to show.</param>
    /// <returns>The item view.</returns>
    IItemView ItemView(Item item);

    /// <summary>
    /// Creates a text label.
    /// </summary>
    /// <param name="text">The text in the label.</param>
    /// <returns>The label.</returns>
    ILabel Label(string text);

    /// <summary>
    /// Creates a component that shrinks a GUI component to its minimum size.
    /// </summary>
    /// <param name="inner">The component to shrink.</param>
    /// <returns>The shrinker.</returns>
    IShrinker Shrinker(IGuiComponent inner);

    /// <summary>
    /// Creates a component with simple functionality.
    /// </summary>
    /// <param name="constraints">The constraints for this component</param>
    /// <param name="draw">The action to execute when drawing this component.</param>
    /// <returns>The resulting component.</returns>
    ISimpleComponent SimpleComponent(
        IGuiConstraints constraints,
        Action<SpriteBatch, Rectangle> draw
    );

    /// <summary>
    /// Creates a texture that is stretched to fill a space.
    /// </summary>
    /// <param name="texture">The texture to draw.</param>
    /// <returns>The stretched texture.</returns>
    IStretchedTexture StretchedTexture(Texture2D texture);

    /// <summary>
    /// Creates a standard text box.
    /// </summary>
    /// <param name="state">The state of the component.</param>
    /// <param name="inputHelper">The input helper to use for tracking input.</param>
    /// <returns>The text box.</returns>
    ITextBox TextBox(ITextInput.IState state, IInputHelper inputHelper);

    /// <summary>
    /// Creates a single-line text input component.
    /// </summary>
    /// <param name="state">The state of the component.</param>
    /// <param name="inputHelper">The input helper to use for tracking input.</param>
    /// <returns>The text input.</returns>
    ITextInput TextInput(ITextInput.IState state, IInputHelper inputHelper);

    /// <summary>
    /// Creates a component which fills a space with a texture created from a 3x3 grid.
    /// </summary>
    /// <param name="texture">The source texture.</param>
    /// <param name="topLeft">The source rectangle of the top left corner, if any.</param>
    /// <param name="topCenter">The source rectangle of the top center edge, if any.</param>
    /// <param name="topRight">The source rectangle of the top right corner, if any.</param>
    /// <param name="centerLeft">The source rectangle of the center left edge, if any.</param>
    /// <param name="center">The source rectangle of the center, if any.</param>
    /// <param name="centerRight">The source rectangle of the center right edge, if any.</param>
    /// <param name="bottomLeft">The source rectangle of the bottom left corner, if any.</param>
    /// <param name="bottomCenter">The source rectangle of the bottom center edge, if any.</param>
    /// <param name="bottomRight">The source rectangle of the bottom right corner, if any.</param>
    /// <returns>The grid texture component.</returns>
    ITextureBox TextureBox(
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
    );

    /// <summary>
    /// Creates a vertically scrollable view that clips its inner component.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="state">The state of the component.</param>
    /// <returns>The vertical scroll area.</returns>
    IVerticalScrollArea VerticalScrollArea(IGuiComponent inner, IVerticalScrollbar.IState state);

    /// <summary>
    /// Creates a new horizontal layout.
    /// </summary>
    /// <param name="components">The components that are a part of this layout.</param>
    /// <returns>The horizontal layout.</returns>
    IHorizontalLayout HorizontalLayout(IEnumerable<IGuiComponent> components);

    /// <summary>
    /// Creates a new vertical layout.
    /// </summary>
    /// <param name="components">The components that are a part of this layout.</param>
    /// <returns>The vertical layout.</returns>
    IVerticalLayout VerticalLayout(IEnumerable<IGuiComponent> components);

    /// <summary>
    /// Creates a new <see cref="IClickableMenu"/> from a component.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="helper">The helper to use.</param>
    /// <returns>The menu.</returns>
    IClickableMenu ComponentMenu(IGuiComponent inner, IModHelper helper);
}

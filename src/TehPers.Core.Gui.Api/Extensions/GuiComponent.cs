using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;

namespace TehPers.Core.Gui.Api.Extensions;

/// <summary>
/// Extension methods for creating GUIs using the base components.
/// </summary>
public static class GuiComponent
{
    private static IDefaultComponentProvider? componentProvider;

    private static IDefaultComponentProvider GetComponentProvider(IGuiBuilder builder)
    {
        if (GuiComponent.componentProvider is not null)
        {
            return GuiComponent.componentProvider;
        }

        if (builder.TryGetExtension(ModInitializer.extensionUniqueId) is
            IDefaultComponentProvider provider)
        {
            GuiComponent.componentProvider = provider;
            return GuiComponent.componentProvider;
        }

        throw new("Base component provider was not found.");
    }

    /// <summary>
    /// Aligns this component in its parent.
    /// </summary>
    /// <param name="inner">The component to align.</param>
    /// <param name="horizontal">The horizontal alignment to apply.</param>
    /// <param name="vertical">The vertical alignment to apply.</param>
    /// <returns>The aligned component.</returns>
    public static IAligner Aligned(
        this IGuiComponent inner,
        HorizontalAlignment? horizontal = null,
        VerticalAlignment? vertical = null
    )
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.Aligner(inner, horizontal, vertical);
    }

    /// <summary>
    /// Creates a new checkbox.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="state">The state of the checkbox.</param>
    /// <returns>The checkbox.</returns>
    public static ICheckbox Checkbox(this IGuiBuilder builder, ICheckbox.IState state)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.Checkbox(state);
    }

    /// <summary>
    /// Clips this component, removing its minimum size constraint. This constrains its
    /// rendering area and mouse inputs if it is shrunk.
    /// </summary>
    /// <param name="inner">The component to clip.</param>
    /// <returns>The clipped component.</returns>
    public static IClipper Clipped(this IGuiComponent inner)
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.Clipper(inner);
    }

    /// <summary>
    /// Further constrains a component's size. This cannot be used to remove constraints.
    /// </summary>
    /// <param name="inner">The component to constrain.</param>
    /// <returns>The constrained component.</returns>
    public static IConstrainer Constrained(this IGuiComponent inner)
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.Constrainer(inner);
    }

    /// <summary>
    /// Creates a new dialogue box component.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <returns>The new dialogue box component.</returns>
    public static IDialogueBox DialogueBox(this IGuiBuilder builder)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.DialogueBox();
    }

    /// <summary>
    /// Creates a new dropdown component.
    /// </summary>
    /// <typeparam name="T">The type of items that can be chosen.</typeparam>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="state">The state of the dropdown component.</param>
    /// <returns>The dropdown component.</returns>
    public static IDropdown<T> Dropdown<T>(this IGuiBuilder builder, IDropdown<T>.IState state)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.Dropdown(state);
    }

    /// <summary>
    /// Creates a new empty component. This can stretch to any size and fill any space.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <returns>The empty component.</returns>
    public static IGuiComponent Empty(this IGuiBuilder builder)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.EmptySpace();
    }

    /// <summary>
    /// Creates a new component which renders an item.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="item">The item to show in this view.</param>
    /// <returns>The item view component.</returns>
    public static IItemView ItemView(this IGuiBuilder builder, Item item)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.ItemView(item);
    }

    /// <summary>
    /// Creates a new text label.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="text">The text in the label.</param>
    /// <returns>The label.</returns>
    public static ILabel Label(this IGuiBuilder builder, string text)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.Label(text);
    }


    /// <summary>
    /// Creates a new menu background component.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="layerDepth">The layer depth to draw the component on.</param>
    /// <returns>The menu background component.</returns>
    public static IGuiComponent MenuBackground(
        this IGuiBuilder builder,
        float? layerDepth = null
    )
    {
        return builder.TextureBox(
                Game1.menuTexture,
                new(0, 0, 64, 64),
                new(128, 0, 64, 64),
                new(192, 0, 64, 64),
                new(0, 128, 64, 64),
                null,
                new(192, 128, 64, 64),
                new(0, 192, 64, 64),
                new(128, 192, 64, 64),
                new(192, 192, 64, 64)
            )
            .WithLayerDepth(layerDepth ?? 0)
            .WithBackground(
                builder.Texture(Game1.menuTexture)
                    .WithSourceRectangle(new(64, 128, 64, 64))
                    .WithMinScale(minScale: GuiSize.Zero)
                    .WithMaxScale(PartialGuiSize.Empty)
                    .WithLayerDepth(layerDepth ?? 0)
                    .WithPadding(32)
            );
    }

    /// <summary>
    /// Creates a new horizontal menu separator component.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="layerDepth">The layer depth to draw the component on.</param>
    /// <returns>The menu horizontal separator component.</returns>
    public static IGuiComponent MenuHorizontalSeparator(
        this IGuiBuilder builder,
        float? layerDepth = null
    )
    {
        return builder.HorizontalLayout(
            builder.Texture(Game1.menuTexture)
                .WithMinScale(GuiSize.One)
                .WithMaxScale(PartialGuiSize.One)
                .WithSourceRectangle(new(0, 64, 64, 64))
                .WithLayerDepth(layerDepth ?? 0),
            builder.Texture(Game1.menuTexture)
                .WithMinScale(GuiSize.One)
                .WithMaxScale(new PartialGuiSize(null, 1))
                .WithSourceRectangle(new(128, 64, 64, 64))
                .WithLayerDepth(layerDepth ?? 0),
            builder.Texture(Game1.menuTexture)
                .WithMinScale(GuiSize.One)
                .WithMaxScale(PartialGuiSize.One)
                .WithSourceRectangle(new(192, 64, 64, 64))
                .WithLayerDepth(layerDepth ?? 0)
        );
    }

    /// <summary>
    /// Creates a new vertical menu separator component.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="topConnector">The end T-connector to put at the top of this separator.</param>
    /// <param name="bottomConnector">The end T-connector to put at the bottom of this separator.</param>
    /// <param name="layerDepth">The layer depth to draw the component on.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IGuiComponent MenuVerticalSeparator(
        this IGuiBuilder builder,
        MenuSeparatorConnector topConnector = MenuSeparatorConnector.MenuBorder,
        MenuSeparatorConnector bottomConnector = MenuSeparatorConnector.MenuBorder,
        float? layerDepth = null
    )
    {
        return builder.VerticalLayout(
            layout =>
            {
                // Top connector
                switch (topConnector)
                {
                    case MenuSeparatorConnector.PinMenuBorder:
                        builder.Texture(Game1.menuTexture)
                            .WithSourceRectangle(new(64, 0, 64, 64))
                            .WithMinScale(GuiSize.One)
                            .WithMaxScale(PartialGuiSize.One)
                            .WithLayerDepth(layerDepth ?? 0)
                            .AddTo(layout);
                        break;
                    case MenuSeparatorConnector.MenuBorder:
                        builder.Texture(Game1.menuTexture)
                            .WithSourceRectangle(new(0, 704, 64, 64))
                            .WithMinScale(GuiSize.One)
                            .WithMaxScale(PartialGuiSize.One)
                            .WithLayerDepth(layerDepth ?? 0)
                            .AddTo(layout);
                        break;
                    case MenuSeparatorConnector.Separator:
                        builder.Texture(Game1.menuTexture)
                            .WithSourceRectangle(new(192, 896, 64, 64))
                            .WithMinScale(GuiSize.One)
                            .WithMaxScale(PartialGuiSize.One)
                            .WithLayerDepth(layerDepth ?? 0)
                            .AddTo(layout);
                        break;
                    case MenuSeparatorConnector.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(topConnector),
                            topConnector,
                            "Unknown connector"
                        );
                }

                // Body
                builder.Texture(Game1.menuTexture)
                    .WithSourceRectangle(new(64, 64, 64, 64))
                    .WithMinScale(GuiSize.One)
                    .WithMaxScale(new PartialGuiSize(1, null))
                    .WithLayerDepth(layerDepth ?? 0)
                    .AddTo(layout);

                // Bottom connector
                switch (bottomConnector)
                {
                    case MenuSeparatorConnector.PinMenuBorder:
                        builder.Texture(Game1.menuTexture)
                            .WithSourceRectangle(new(64, 192, 64, 64))
                            .WithMinScale(GuiSize.One)
                            .WithMaxScale(PartialGuiSize.One)
                            .WithLayerDepth(layerDepth ?? 0)
                            .AddTo(layout);
                        break;
                    case MenuSeparatorConnector.MenuBorder:
                        builder.Texture(Game1.menuTexture)
                            .WithSourceRectangle(new(128, 960, 64, 64))
                            .WithMinScale(GuiSize.One)
                            .WithMaxScale(PartialGuiSize.One)
                            .WithLayerDepth(layerDepth ?? 0)
                            .AddTo(layout);
                        break;
                    case MenuSeparatorConnector.Separator:
                        builder.Texture(Game1.menuTexture)
                            .WithSourceRectangle(new(192, 576, 64, 64))
                            .WithMinScale(GuiSize.One)
                            .WithMaxScale(PartialGuiSize.One)
                            .WithLayerDepth(layerDepth ?? 0)
                            .AddTo(layout);
                        break;
                    case MenuSeparatorConnector.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(bottomConnector),
                            bottomConnector,
                            "Unknown connector"
                        );
                }
            }
        );
    }

    /// <summary>
    /// Creates a new click detector that executes an action when the inner component is clicked.
    /// </summary>
    /// <param name="inner">The component to detect clicks for.</param>
    /// <param name="action">The action to execute on click.</param>
    /// <returns>The click detector.</returns>
    public static IClickDetector OnClick(this IGuiComponent inner, Action<ClickType> action)
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.ClickDetector(inner, action);
    }

    /// <summary>
    /// Creates a new click detector that executes an action when the inner component is clicked.
    /// </summary>
    /// <param name="inner">The component to detect clicks for.</param>
    /// <param name="clickType">The type of click to watch for.</param>
    /// <param name="action">The action to execute on click.</param>
    /// <returns>The click detector.</returns>
    public static IClickDetector OnClick(
        this IGuiComponent inner,
        ClickType clickType,
        Action action
    )
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.ClickDetector(
            inner,
            c =>
            {
                if (c == clickType)
                {
                    action();
                }
            }
        );
    }

    /// <summary>
    /// Executes an action when this control is hovered.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="action">The action to perform.</param>
    /// <returns>The component, wrapped by a hover detector.</returns>
    public static IHoverDetector OnHover(this IGuiComponent inner, Action action)
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.HoverDetector(inner, action);
    }

    /// <summary>
    /// Shrinks this component to its minimum size.
    /// </summary>
    /// <param name="inner">The component to shrink.</param>
    /// <returns>The shrunk component.</returns>
    public static IShrinker Shrink(this IGuiComponent inner)
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.Shrinker(inner);
    }

    /// <summary>
    /// Creates a new component with simple functionality.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="constraints">The constraints on the component.</param>
    /// <param name="draw">The action which draws the component.</param>
    /// <returns>The simple component.</returns>
    public static ISimpleComponent SimpleComponent(
        this IGuiBuilder builder,
        GuiConstraints constraints,
        Action<SpriteBatch, Rectangle> draw
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.SimpleComponent(constraints, draw);
    }

    /// <summary>
    /// Creates a new text box.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="state">The state of the text input.</param>
    /// <param name="inputHelper">The input helper.</param>
    /// <returns>The text box component.</returns>
    public static IGuiComponent TextBox(
        this IGuiBuilder builder,
        ITextInput.IState state,
        IInputHelper inputHelper
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.TextBox(state, inputHelper);
    }

    /// <summary>
    /// Creates a new single-line text input.
    /// </summary>
    /// <returns>The label component.</returns>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="state">The state of the text input.</param>
    /// <param name="inputHelper">The input helper.</param>
    /// <returns>The text input component.</returns>
    public static ITextInput TextInput(
        this IGuiBuilder builder,
        ITextInput.IState state,
        IInputHelper inputHelper
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.TextInput(state, inputHelper);
    }

    /// <summary>
    /// Creates a new stretchable texture component.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="texture">The texture to draw.</param>
    /// <returns>The texture component.</returns>
    public static IStretchedTexture Texture(this IGuiBuilder builder, Texture2D texture)
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.StretchedTexture(texture);
    }

    /// <summary>
    /// Fills a space with a texture created from a 3x3 grid, where the sides and center can be
    /// stretched.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
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
    public static ITextureBox TextureBox(
        this IGuiBuilder builder,
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
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.TextureBox(
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

    /// <summary>
    /// Fills a space with a texture created from a 3x3 grid, where the sides and center can be
    /// stretched.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="texture">The source texture.</param>
    /// <param name="sourceRectangle">
    /// The source rectangle of the texture box. The rectangle will be split evenly into a 3x3
    /// grid, and the sides and center of the grid will be stretched as needed. As a result,
    /// the dimensions of this rectangle must be multiples of 3.
    /// </param>
    /// <returns>The grid texture component.</returns>
    public static ITextureBox TextureBox(
        this IGuiBuilder builder,
        Texture2D texture,
        Rectangle sourceRectangle
    )
    {
        // Get dimensions
        var cellWidth = Math.DivRem(sourceRectangle.Width, 3, out var widthRem);
        if (widthRem != 0)
        {
            throw new ArgumentException(
                "The source rectangle's dimensions must be multiples of 3.",
                nameof(sourceRectangle)
            );
        }

        var cellHeight = Math.DivRem(sourceRectangle.Height, 3, out var heightRem);
        if (heightRem != 0)
        {
            throw new ArgumentException(
                "The source rectangle's dimensions must be multiples of 3.",
                nameof(sourceRectangle)
            );
        }

        var x = sourceRectangle.X;
        var y = sourceRectangle.Y;
        return builder.TextureBox(
            texture,
            new(x, y, cellWidth, cellHeight),
            new(x + cellWidth, y, cellWidth, cellHeight),
            new(x + 2 * cellWidth, y, cellWidth, cellHeight),
            new(x, y + cellHeight, cellWidth, cellHeight),
            new(x + cellWidth, y + cellHeight, cellWidth, cellHeight),
            new(x + 2 * cellWidth, y + cellHeight, cellWidth, cellHeight),
            new(x, y + 2 * cellHeight, cellWidth, cellHeight),
            new(x + cellWidth, y + 2 * cellHeight, cellWidth, cellHeight),
            new(x + 2 * cellWidth, y + 2 * cellHeight, cellWidth, cellHeight)
        );
    }

    /// <summary>
    /// Allows this component to be scrolled vertically. This allows the component to be
    /// rendered at any height and allows the user to use the scroll wheel to scroll up and
    /// down the component within the view. This does not add a scrollbar.
    /// </summary>
    /// <param name="inner">The inner component.</param>
    /// <param name="scrollState">The scrolling state.</param>
    /// <returns>The vertically scrollable component.</returns>
    public static IVerticalScrollArea VerticallyScrollable(
        this IGuiComponent inner,
        IVerticalScrollbar.IState scrollState
    )
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.VerticalScrollArea(inner, scrollState);
    }

    /// <summary>
    /// Adds a background to this component.
    /// </summary>
    /// <param name="inner">The component to add a background to.</param>
    /// <param name="background">The background component.</param>
    /// <returns>A component that applies a background to the inner component.</returns>
    public static IGuiComponent WithBackground(
        this IGuiComponent inner,
        IGuiComponent background
    )
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.Background(inner, background);
    }

    /// <summary>
    /// Adds padding to this component.
    /// </summary>
    /// <param name="inner">The component to add padding to.</param>
    /// <param name="left">The amount of padding on the left.</param>
    /// <param name="right">The amount of padding on the right.</param>
    /// <param name="top">The amount of padding on the top.</param>
    /// <param name="bottom">The amount of padding on the bottom.</param>
    /// <returns>The padded component.</returns>
    public static IComponentPadder WithPadding(
        this IGuiComponent inner,
        float left,
        float right,
        float top,
        float bottom
    )
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.Padder(inner, left, right, top, bottom);
    }

    /// <summary>
    /// Adds padding to this component.
    /// </summary>
    /// <param name="inner">The component to add padding to.</param>
    /// <param name="leftRight">The amount of padding on the left and right.</param>
    /// <param name="topBottom">The amount of padding on the top and bottom.</param>
    /// <returns>The padded component.</returns>
    public static IComponentPadder WithPadding(
        this IGuiComponent inner,
        float leftRight,
        float topBottom
    )
    {
        return inner.WithPadding(leftRight, leftRight, topBottom, topBottom);
    }

    /// <summary>
    /// Adds padding to this component.
    /// </summary>
    /// <param name="inner">The component to add padding to.</param>
    /// <param name="sides">The amount of padding on all sides.</param>
    /// <returns>The padded component.</returns>
    public static IComponentPadder WithPadding(this IGuiComponent inner, float sides)
    {
        return inner.WithPadding(sides, sides, sides, sides);
    }

    /// <summary>
    /// Creates a new horizontal layout containing the given components.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="components">The components that are a part of this layout.</param>
    /// <returns>The horizontal layout.</returns>
    public static IHorizontalLayout HorizontalLayout(
        this IGuiBuilder builder,
        IEnumerable<IGuiComponent> components
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.HorizontalLayout(components);
    }

    /// <summary>
    /// Creates a new horizontal layout containing the given components.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="components">The components that are a part of this layout.</param>
    /// <returns>The horizontal layout.</returns>
    public static IHorizontalLayout HorizontalLayout(
        this IGuiBuilder builder,
        params IGuiComponent[] components
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.HorizontalLayout(components);
    }

    /// <summary>
    /// Creates a new horizontal layout containing the given components.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="addComponents">A callback which adds the components.</param>
    /// <returns>The horizontal layout.</returns>
    public static IHorizontalLayout HorizontalLayout(
        this IGuiBuilder builder,
        Action<ILayoutBuilder> addComponents
    )
    {
        var components = new List<IGuiComponent>();
        addComponents(new LayoutBuilder(components));

        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.HorizontalLayout(components);
    }

    /// <summary>
    /// Creates a new vertical layout containing the given components.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="components">The components that are a part of this layout.</param>
    /// <returns>The vertical layout.</returns>
    public static IVerticalLayout VerticalLayout(
        this IGuiBuilder builder,
        IEnumerable<IGuiComponent> components
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.VerticalLayout(components);
    }

    /// <summary>
    /// Creates a new vertical layout containing the given components.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="components">The components that are a part of this layout.</param>
    /// <returns>The vertical layout.</returns>
    public static IVerticalLayout VerticalLayout(
        this IGuiBuilder builder,
        params IGuiComponent[] components
    )
    {
        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.VerticalLayout(components);
    }

    /// <summary>
    /// Creates a new vertical layout containing the given components.
    /// </summary>
    /// <param name="builder">The GUI builder.</param>
    /// <param name="addComponents">A callback which adds the components.</param>
    /// <returns>The vertical layout.</returns>
    public static IVerticalLayout VerticalLayout(
        this IGuiBuilder builder,
        Action<ILayoutBuilder> addComponents
    )
    {
        var components = new List<IGuiComponent>();
        addComponents(new LayoutBuilder(components));

        var provider = GuiComponent.GetComponentProvider(builder);
        return provider.VerticalLayout(components);
    }

    /// <summary>
    /// Converts this component to an <see cref="IClickableMenu"/>.
    /// </summary>
    /// <param name="inner">The component to turn into a menu.</param>
    /// <param name="helper">The helper to use.</param>
    /// <returns>The menu.</returns>
    public static IClickableMenu ToMenu(this IGuiComponent inner, IModHelper helper)
    {
        var provider = GuiComponent.GetComponentProvider(inner.GuiBuilder);
        return provider.ComponentMenu(inner, helper);
    }

    private class LayoutBuilder : ILayoutBuilder
    {
        private readonly List<IGuiComponent> components;

        public LayoutBuilder(List<IGuiComponent> components)
        {
            this.components = components;
        }

        public void Add(IGuiComponent component)
        {
            this.components.Add(component);
        }
    }
}

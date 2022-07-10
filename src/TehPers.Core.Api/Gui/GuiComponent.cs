using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TehPers.Core.Api.Gui.Components;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Gui.States;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Extension methods for components.
    /// </summary>
    public static class GuiComponent
    {
        /// <summary>
        /// Converts this component to an <see cref="StardewValley.Menus.IClickableMenu"/>.
        /// </summary>
        /// <param name="component">The component to turn into a menu.</param>
        /// <param name="helper">The helper to use.</param>
        /// <returns>The menu.</returns>
        public static StardewValley.Menus.IClickableMenu ToMenu(
            this IGuiComponent component,
            IModHelper helper
        )
        {
            return new SimpleManagedMenu(component, helper);
        }

        /// <summary>
        /// Adds a background to this component.
        /// </summary>
        /// <param name="component">The component to add a background to.</param>
        /// <param name="background">The background component.</param>
        /// <returns>A component that applies a background to the inner component.</returns>
        public static IGuiComponent WithBackground(
            this IGuiComponent component,
            IGuiComponent background
        )
        {
            return new WithBackground(component, background);
        }

        /// <summary>
        /// Horizontally aligns this component in its parent.
        /// </summary>
        /// <param name="component">The component to horizontally align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static IGuiComponent Aligned(
            this IGuiComponent component,
            HorizontalAlignment horizontal
        )
        {
            return new HorizontalAligner(component, horizontal);
        }

        /// <summary>
        /// Vertically aligns this component in its parent.
        /// </summary>
        /// <param name="component">The component to vertically align.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static IGuiComponent Aligned(
            this IGuiComponent component,
            VerticalAlignment vertical
        )
        {
            return new VerticalAligner(component, vertical);
        }

        /// <summary>
        /// Fully aligns this component in its parent.
        /// </summary>
        /// <param name="component">The component to align.</param>
        /// <param name="horizontal">The horizontal alignment to apply.</param>
        /// <param name="vertical">The vertical alignment to apply.</param>
        /// <returns>The aligned component.</returns>
        public static IGuiComponent Aligned(
            this IGuiComponent component,
            HorizontalAlignment horizontal,
            VerticalAlignment vertical
        )
        {
            return component.Aligned(horizontal).Aligned(vertical);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="sides">The amount of padding on all sides.</param>
        /// <returns>The padded component.</returns>
        public static IGuiComponent WithPadding(this IGuiComponent component, float sides)
        {
            return component.WithPadding(sides, sides, sides, sides);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="leftRight">The amount of padding on the left and right.</param>
        /// <param name="topBottom">The amount of padding on the top and bottom.</param>
        /// <returns>The padded component.</returns>
        public static IGuiComponent WithPadding(
            this IGuiComponent component,
            float leftRight,
            float topBottom
        )
        {
            return component.WithPadding(leftRight, leftRight, topBottom, topBottom);
        }

        /// <summary>
        /// Adds padding to this component.
        /// </summary>
        /// <param name="component">The component to add padding to.</param>
        /// <param name="left">The amount of padding on the left.</param>
        /// <param name="right">The amount of padding on the right.</param>
        /// <param name="top">The amount of padding on the top.</param>
        /// <param name="bottom">The amount of padding on the bottom.</param>
        /// <returns>The padded component.</returns>
        public static IGuiComponent WithPadding(
            this IGuiComponent component,
            float left,
            float right,
            float top,
            float bottom
        )
        {
            return new ComponentPadder(component, left, right, top, bottom);
        }

        /// <summary>
        /// Shrinks this component to its minimum size.
        /// </summary>
        /// <param name="component">The component to shrink.</param>
        /// <returns>The shrunk component.</returns>
        public static IGuiComponent Shrink(this IGuiComponent component)
        {
            return new Shrink(component);
        }

        /// <summary>
        /// Executes an action when this control is clicked.
        /// </summary>
        /// <param name="component">The inner component.</param>
        /// <param name="onClick">The action to perform.</param>
        /// <returns>The component, wrapped by a click detector.</returns>
        public static IGuiComponent OnClick(this IGuiComponent component, Action<ClickType> onClick)
        {
            return new ClickDetector(component, onClick);
        }

        /// <summary>
        /// Executes an action when this control is clicked.
        /// </summary>
        /// <param name="component">The inner component.</param>
        /// <param name="clickType">The type of click to detect.</param>
        /// <param name="onClick">The action to perform.</param>
        /// <returns>The component, wrapped by a click detector.</returns>
        public static IGuiComponent OnClick(this IGuiComponent component, ClickType clickType, Action onClick)
        {
            return component.OnClick(ct =>
            {
                if (clickType == ct)
                {
                    onClick();
                }
            });
        }

        /// <summary>
        /// Executes an action when this control is hovered.
        /// </summary>
        /// <param name="component">The inner component.</param>
        /// <param name="onHover">The action to perform.</param>
        /// <returns>The component, wrapped by a hover detector.</returns>
        public static IGuiComponent OnHover(this IGuiComponent component, Action onHover)
        {
            return new HoverDetector(component, onHover);
        }

        /// <summary>
        /// Further constrains a component's size. This cannot be used to remove constraints.
        /// </summary>
        /// <param name="component">The component to constrain.</param>
        /// <param name="minSize">The additional minimum size constraints, if any.</param>
        /// <param name="maxSize">The additional maximum size constraints, if any.</param>
        /// <returns>The constrained component.</returns>
        public static IGuiComponent Constrained(
            this IGuiComponent component,
            PartialGuiSize? minSize = null,
            PartialGuiSize? maxSize = null
        )
        {
            return new Constrainer(component)
            {
                MinSize = minSize ?? PartialGuiSize.Empty,
                MaxSize = maxSize ?? PartialGuiSize.Empty,
            };
        }

        /// <summary>
        /// Clips this component, removing its minimum size constraint. This constrains its
        /// rendering area and mouse inputs if it is shrunk.
        /// </summary>
        /// <param name="component">The component to clip.</param>
        /// <returns>The clipped component.</returns>
        public static IGuiComponent Clipped(this IGuiComponent component)
        {
            return new Clipper(component);
        }

        /// <summary>
        /// Creates a new text label.
        /// 
        /// </summary>
        /// <param name="text">The text in the label.</param>
        /// <param name="font">The font to render the text with.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="scale">The scale to apply to the text.</param>
        /// <param name="spriteEffects">The sprite effects to apply to the text.</param>
        /// <param name="layerDepth">The layer depth to render the text at.</param>
        /// <returns>The label component.</returns>
        public static IGuiComponent Label(
            string text,
            SpriteFont? font = null,
            Color? color = null,
            Vector2? scale = null,
            SpriteEffects? spriteEffects = null,
            float? layerDepth = null
        )
        {
            return new Label(text).MaybeInitRef(font, (l, f) => l with { Font = f })
                .MaybeInitVal(color, (l, c) => l with { Color = c })
                .MaybeInitVal(scale, (l, s) => l with { Scale = s })
                .MaybeInitVal(spriteEffects, (l, s) => l with { SpriteEffects = s })
                .MaybeInitVal(layerDepth, (l, d) => l with { LayerDepth = d });
        }

        /// <summary>
        /// Creates a new text label.
        /// </summary>
        /// <returns>The label component.</returns>
        /// <param name="state">The state of the text input.</param>
        /// <param name="inputHelper">The input helper.</param>
        /// <param name="font">The font of the text in the input.</param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="cursorColor">The color of the cursor.</param>
        /// <param name="highlightedTextColor">The color of the highlighted text.</param>
        /// <param name="highlightedTextBackgroundColor">The background color of the highlighted text.</param>
        /// <param name="unfocusOnEsc">Whether this component should lose focus when ESC is received.</param>
        /// <param name="insertionCue">The sound cue to play when text is typed and no specific cue overrides it.</param>
        /// <param name="overrideInsertionCues">The sound cue to play when specific characters are typed instead.</param>
        /// <param name="deletionCue">The sound cue to play when text is deleted.</param>
        /// <returns>The text input component.</returns>
        public static IGuiComponent TextInput(
            TextInputState state,
            IInputHelper inputHelper,
            SpriteFont? font = null,
            Color? textColor = null,
            Color? cursorColor = null,
            Color? highlightedTextColor = null,
            Color? highlightedTextBackgroundColor = null,
            bool? unfocusOnEsc = null,
            string? insertionCue = null,
            ImmutableDictionary<char, string?>? overrideInsertionCues = null,
            string? deletionCue = null
        )
        {
            return new TextInput(state, inputHelper)
                .MaybeInitRef(font, (input, f) => input with { Font = f })
                .MaybeInitVal(textColor, (input, c) => input with { TextColor = c })
                .MaybeInitVal(cursorColor, (input, c) => input with { CursorColor = c })
                .MaybeInitVal(
                    highlightedTextColor,
                    (input, c) => input with { HighlightedTextColor = c }
                )
                .MaybeInitVal(
                    highlightedTextBackgroundColor,
                    (input, c) => input with { HighlightedTextBackgroundColor = c }
                )
                .MaybeInitVal(unfocusOnEsc, (input, b) => input with { UnfocusOnEsc = b })
                .MaybeInitRef(insertionCue, (input, s) => input with { InsertionCue = s })
                .MaybeInitRef(
                    overrideInsertionCues,
                    (input, d) => input with { OverrideInsertionCues = d }
                )
                .MaybeInitRef(deletionCue, (input, s) => input with { DeletionCue = s });
        }

        /// <summary>
        /// Creates a new textbox.
        /// </summary>
        /// <param name="state">The state of the text input.</param>
        /// <param name="inputHelper">The input helper.</param>
        /// <returns>The textbox component.</returns>
        public static IGuiComponent TextBox(TextInputState state, IInputHelper inputHelper)
        {
            return new TextBox(state, inputHelper);
        }

        /// <summary>
        /// Creates a new textbox.
        /// </summary>
        /// <param name="input">The text input component.</param>
        /// <returns>The textbox component.</returns>
        public static IGuiComponent TextBox(IGuiComponent input)
        {
            return new TextBox(input);
        }

        /// <summary>
        /// Creates a new stretchable texture component. To stop the texture from stretching, set
        /// the <paramref name="minScale"/> to <see cref="GuiSize.One"/> and the
        /// <paramref name="maxScale"/> to <see cref="PartialGuiSize.One"/>.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="sourceRectangle">The source rectangle on the texture.</param>
        /// <param name="color">The color to tint the texture.</param>
        /// <param name="effects">The sprite effects to apply to the texture.</param>
        /// <param name="layerDepth">The layer depth to draw the background on.</param>
        /// <param name="minScale">The minimum scaled size of this texture.</param>
        /// <param name="maxScale">The maximum scaled size of this texture.</param>
        /// <returns>The texture component.</returns>
        public static IGuiComponent Texture(
            Texture2D texture,
            Rectangle? sourceRectangle = null,
            Color? color = null,
            SpriteEffects? effects = null,
            float? layerDepth = null,
            GuiSize? minScale = null,
            PartialGuiSize? maxScale = null
        )
        {
            return new StretchedTexture(texture)
                .MaybeInitVal(sourceRectangle, (t, s) => t with { SourceRectangle = s })
                .MaybeInitVal(color, (t, c) => t with { Color = c })
                .MaybeInitVal(effects, (t, e) => t with { Effects = e })
                .MaybeInitVal(layerDepth, (t, d) => t with { LayerDepth = d })
                .MaybeInitRef(minScale, (t, s) => t with { MinScale = s })
                .MaybeInitRef(maxScale, (t, s) => t with { MaxScale = s });
        }

        /// <summary>
        /// Creates a new component which renders an item.
        /// </summary>
        /// <param name="item">The item to show in this view.</param>
        /// <param name="transparency">The transparency of the item.</param>
        /// <param name="layerDepth">The layer depth to draw at.</param>
        /// <param name="sideLength">The side length of this item view.</param>
        /// <param name="drawStackNumber">How to draw the stack number, if any.</param>
        /// <param name="color">The color to tint the item.</param>
        /// <param name="drawShadow">Whether to draw the item's shadow.</param>
        /// <returns>The item view component.</returns>
        public static IGuiComponent ItemView(
            Item item,
            float? transparency = null,
            float? layerDepth = null,
            float? sideLength = null,
            StackDrawType? drawStackNumber = null,
            Color? color = null,
            bool? drawShadow = null
        )
        {
            return new ItemView(item)
                .MaybeInitVal(transparency, (i, t) => i with { Transparency = t })
                .MaybeInitVal(layerDepth, (i, d) => i with { LayerDepth = d })
                .MaybeInitVal(sideLength, (i, s) => i with { SideLength = s })
                .MaybeInitVal(drawStackNumber, (i, d) => i with { DrawStackNumber = d })
                .MaybeInitVal(color, (i, c) => i with { Color = c })
                .MaybeInitVal(drawShadow, (i, d) => i with { DrawShadow = d });
        }

        /// <summary>
        /// Creates a new empty component. This can stretch to any size and just fills space.
        /// </summary>
        /// <returns>The empty component.</returns>
        public static IGuiComponent Empty()
        {
            return new EmptySpace();
        }

        /// <summary>
        /// Creates a new dropdown component.
        /// </summary>
        /// <typeparam name="T">The type of items that can be chosen.</typeparam>
        /// <param name="state">The state of the dropdown component.</param>
        /// <param name="layerDepth">The layer depth the component should be rendered at.</param>
        /// <returns>The dropdown component.</returns>
        public static IGuiComponent Dropdown<T>(DropdownState<T> state, float? layerDepth = null)
        {
            // TODO
            return new Dropdown<T>(state)
                .MaybeInitVal(layerDepth, (d, l) => d with { LayerDepth = l });
        }

        /// <summary>
        /// Creates a new component with simple functionality.
        /// </summary>
        /// <param name="constraints">The constraints on the component.</param>
        /// <param name="draw">A callback which draws the component.</param>
        /// <returns>The new component.</returns>
        public static IGuiComponent Simple(
            GuiConstraints constraints,
            Action<SpriteBatch, Rectangle> draw
        )
        {
            return new SimpleComponent(constraints, draw);
        }

        /// <summary>
        /// Fills a space with a texture created from a 3x3 grid, where the sides and center can be
        /// stretched.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="sourceRectangle">
        /// The source rectangle of the texture box. The rectangle will be split evenly into a 3x3
        /// grid, and the sides and center of the grid will be stretched as needed. As a result,
        /// the dimensions of this rectangle must be multiples of 3.
        /// </param>
        /// <param name="minScale">The minimum scale of the individual texture parts.</param>
        /// <param name="layerDepth">The layer depth to draw it at.</param>
        /// <returns>The grid texture component.</returns>
        public static IGuiComponent TextureBox(
            Texture2D texture,
            Rectangle sourceRectangle,
            GuiSize? minScale = null,
            float? layerDepth = null
        )
        {
            // Get dimensions
            var cellWidth = Math.DivRem(sourceRectangle.Width, 3, out var widthRem);
            if (widthRem != 0)
            {
                throw new ArgumentException("The source rectangle's dimensions must be multiples of 3.", nameof(sourceRectangle));
            }
            var cellHeight = Math.DivRem(sourceRectangle.Height, 3, out var heightRem);
            if (heightRem != 0)
            {
                throw new ArgumentException("The source rectangle's dimensions must be multiples of 3.", nameof(sourceRectangle));
            }

            var x = sourceRectangle.X;
            var y = sourceRectangle.Y;
            return GuiComponent.TextureBox(
                texture,
                new(x, y, cellWidth, cellHeight),
                new(x + cellWidth, y, cellWidth, cellHeight),
                new(x + 2 * cellWidth, y, cellWidth, cellHeight),
                new(x, y + cellHeight, cellWidth, cellHeight),
                new(x + cellWidth, y + cellHeight, cellWidth, cellHeight),
                new(x + 2 * cellWidth, y + cellHeight, cellWidth, cellHeight),
                new(x, y + 2 * cellHeight, cellWidth, cellHeight),
                new(x + cellWidth, y + 2 * cellHeight, cellWidth, cellHeight),
                new(x + 2 * cellWidth, y + 2 * cellHeight, cellWidth, cellHeight),
                minScale,
                layerDepth
            );
        }

        /// <summary>
        /// Fills a space with a texture created from a 3x3 grid, where the sides and center can be
        /// stretched.
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
        /// <param name="minScale">The minimum scale of the individual texture parts.</param>
        /// <param name="layerDepth">The layer depth to draw it at.</param>
        /// <returns>The grid texture component.</returns>
        public static IGuiComponent TextureBox(
            Texture2D texture,
            Rectangle? topLeft,
            Rectangle? topCenter,
            Rectangle? topRight,
            Rectangle? centerLeft,
            Rectangle? center,
            Rectangle? centerRight,
            Rectangle? bottomLeft,
            Rectangle? bottomCenter,
            Rectangle? bottomRight,
            GuiSize? minScale = null,
            float? layerDepth = null
        )
        {
            return new TextureBox(
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
            )
            .MaybeInitRef(minScale, (t, s) => t with { MinScale = s })
            .MaybeInitVal(layerDepth, (t, d) => t with { LayerDepth = d });
        }

        /// <summary>
        /// Creates a new menu background component.
        /// </summary>
        /// <param name="layerDepth">The layer depth to draw the component on.</param>
        /// <returns>The menu background component.</returns>
        public static IGuiComponent MenuBackground(float? layerDepth = null)
        {
            return GuiComponent.TextureBox(
                Game1.menuTexture,
                new(0, 0, 64, 64),
                new(128, 0, 64, 64),
                new(192, 0, 64, 64),
                new(0, 128, 64, 64),
                null,
                new(192, 128, 64, 64),
                new(0, 192, 64, 64),
                new(128, 192, 64, 64),
                new(192, 192, 64, 64),
                layerDepth: layerDepth
            )
            .WithBackground(
                GuiComponent.Texture(
                    Game1.menuTexture,
                    sourceRectangle: new(64, 128, 64, 64),
                    minScale: GuiSize.Zero,
                    maxScale: PartialGuiSize.Empty,
                    layerDepth: layerDepth
                )
                .WithPadding(32)
            );
        }

        /// <summary>
        /// Creates a new menu horizontal separator component.
        /// </summary>
        /// <param name="layerDepth">The layer depth to draw the component on.</param>
        /// <returns>The menu horizontal separator component.</returns>
        public static IGuiComponent MenuHorizontalSeparator(float? layerDepth = null)
        {
            return new HorizontalSeparator().MaybeInitVal(
                layerDepth,
                (m, d) => m with { LayerDepth = d }
            );
        }

        /// <summary>
        /// Creates a new dialogue box component.
        /// </summary>
        /// <param name="speaker">The portrait to show, if any.</param>
        /// <param name="drawOnlyBox">Whether to only draw the dialogue box itself.</param>
        /// <param name="message">The message to show in the dialogue box.</param>
        /// <returns>The new dialogue box component.</returns>
        public static IGuiComponent DialogueBox(
            DialogueSpeakerPortrait? speaker = null,
            bool? drawOnlyBox = null,
            string? message = null
        )
        {
            return new DialogueBox().MaybeInitVal(speaker, (d, s) => d with { Speaker = s })
                .MaybeInitVal(drawOnlyBox, (d, b) => d with { DrawOnlyBox = b })
                .MaybeInitRef(message, (d, m) => d with { Message = m });
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The horizontal layout.</returns>
        public static IGuiComponent Horizontal(Action<ILayoutBuilder> addComponents)
        {
            return HorizontalLayout.Build(addComponents);
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components. The components are
        /// automatically aligned as they're added to the layout.
        /// </summary>
        /// <param name="defaultAlignment">The default row alignment.</param>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The horizontal layout.</returns>
        public static IGuiComponent Horizontal(
            VerticalAlignment defaultAlignment,
            Action<ILayoutBuilder> addComponents
        )
        {
            return HorizontalLayout.BuildAligned(addComponents, defaultAlignment);
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The vertical layout.</returns>
        public static IGuiComponent Vertical(Action<ILayoutBuilder> addComponents)
        {
            return VerticalLayout.Build(addComponents);
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components. The components are
        /// automatically aligned as they're added to the layout.
        /// </summary>
        /// <param name="defaultAlignment">The default row alignment.</param>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The vertical layout.</returns>
        public static IGuiComponent Vertical(
            HorizontalAlignment defaultAlignment,
            Action<ILayoutBuilder> addComponents
        )
        {
            return VerticalLayout.BuildAligned(addComponents, defaultAlignment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T MaybeInitRef<T, TProp>(
            this T value,
            TProp? propValue,
            Func<T, TProp, T> init
        )
            where TProp : class
        {
            return propValue is { } v ? init(value, v) : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T MaybeInitVal<T, TProp>(
            this T value,
            TProp? propValue,
            Func<T, TProp, T> init
        )
            where TProp : struct
        {
            return propValue is { } v ? init(value, v) : value;
        }
    }
}

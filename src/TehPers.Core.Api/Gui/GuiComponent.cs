using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Extension methods for components.
    /// </summary>
    public static class GuiComponent
    {
        /// <summary>
        /// Converts this component to an <see cref="IClickableMenu"/>.
        /// </summary>
        /// <param name="component">The component to turn into a menu.</param>
        /// <returns>The menu.</returns>
        public static IClickableMenu ToMenu(this IGuiComponent component)
        {
            return new SimpleManagedMenu(component);
        }

        /// <summary>
        /// Adds a background to this component.
        /// </summary>
        /// <param name="component">The component to add a background to.</param>
        /// <param name="background">The background component.</param>
        /// <returns>A component that applies a background to the inner component.</returns>
        public static WithBackground WithBackground(
            this IGuiComponent component,
            IGuiComponent background
        )
        {
            return new(component, background);
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
            return new HorizontalAlignComponent(component, horizontal);
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
            return new VerticalAlignComponent(component, vertical);
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
            return new PaddedComponent(component, left, right, top, bottom);
        }

        /// <summary>
        /// Shrinks this component to its minimum size.
        /// </summary>
        /// <param name="component">The component to shrink.</param>
        /// <returns>The shrunk component.</returns>
        public static IGuiComponent Shrink(this IGuiComponent component)
        {
            return new ShrinkComponent(component);
        }

        /// <summary>
        /// Executes an action when this control is clicked.
        /// </summary>
        /// <param name="component">The component to check for clicks for.</param>
        /// <param name="onClick">The action to perform.</param>
        /// <returns>The component, wrapped by a click handler.</returns>
        public static IGuiComponent OnClick(this IGuiComponent component, Action<ClickType> onClick)
        {
            return new ClickHandler(component, onClick);
        }

        /// <summary>
        /// Further constrains a component's size.
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
            return new ConstrainedComponent(component)
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
            return new ClippedComponent(component);
        }

        /// <summary>
        /// Creates a new text label.
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
            return new LabelComponent(text).MaybeInitRef(font, (l, f) => l with { Font = f })
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
            return new TextInputComponent(state, inputHelper)
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
            return new TextureComponent(texture)
                .MaybeInitVal(sourceRectangle, (t, s) => t with { SourceRectangle = s })
                .MaybeInitVal(color, (t, c) => t with { Color = c })
                .MaybeInitVal(effects, (t, e) => t with { Effects = e })
                .MaybeInitVal(layerDepth, (t, d) => t with { LayerDepth = d })
                .MaybeInitRef(minScale, (t, s) => t with { MinScale = s })
                .MaybeInitRef(maxScale, (t, s) => t with { MaxScale = s });
        }

        /// <summary>
        /// Creates a new empty component. This can stretch to any size and just fills space.
        /// </summary>
        /// <returns>The empty component.</returns>
        public static IGuiComponent Empty()
        {
            return new EmptyComponent();
        }

        /// <summary>
        /// Creates a new menu background component.
        /// </summary>
        /// <param name="layerDepth">The layer depth to draw the component on.</param>
        /// <returns>The menu background component.</returns>
        public static IGuiComponent MenuBackground(float? layerDepth = null)
        {
            return new MenuBackgroundComponent().MaybeInitVal(
                layerDepth,
                (m, d) => m with { LayerDepth = d }
            );
        }

        /// <summary>
        /// Creates a new menu horizontal separator component.
        /// </summary>
        /// <param name="layerDepth">The layer depth to draw the component on.</param>
        /// <returns>The menu horizontal separator component.</returns>
        public static IGuiComponent MenuHorizontalSeparator(float? layerDepth = null)
        {
            return new MenuHorizontalSeparatorComponent().MaybeInitVal(
                layerDepth,
                (m, d) => m with { LayerDepth = d }
            );
        }

        /// <summary>
        /// Creates a new horizontal layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The horizontal layout.</returns>
        public static IGuiComponent Horizontal(Action<ILayoutBuilder> addComponents)
        {
            return HorizontalLayoutComponent.Build(addComponents);
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
            return HorizontalLayoutComponent.BuildAligned(addComponents, defaultAlignment);
        }

        /// <summary>
        /// Creates a new vertical layout containing the given components.
        /// </summary>
        /// <param name="addComponents">A callback which adds the components.</param>
        /// <returns>The vertical layout.</returns>
        public static IGuiComponent Vertical(Action<ILayoutBuilder> addComponents)
        {
            return VerticalLayoutComponent.Build(addComponents);
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
            return VerticalLayoutComponent.BuildAligned(addComponents, defaultAlignment);
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

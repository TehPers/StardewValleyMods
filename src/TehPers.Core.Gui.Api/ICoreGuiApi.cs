// Copy this file into your mod!

#nullable enable

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;
using TehPers.Core.Gui.Api.Internal;

// ==============================
// Main API types
// ==============================
namespace TehPers.Core.Gui.Api
{
    /// <summary>
    /// An interface for interacting with TehPers.Core.Gui.
    /// </summary>
    public interface ICoreGuiApi
    {
        /// <summary>
        /// Gets the GUI builder.
        /// </summary>
        IGuiBuilder GuiBuilder { get; }

        /// <summary>
        /// Internal use APIs.
        /// </summary>
        IInternalApi InternalApi { get; }
    }

    /// <summary>
    /// A builder for creating GUI components.
    /// </summary>
    public interface IGuiBuilder
    {
        /// <summary>
        /// Tries to add an extension. Extensions are visible to all GUI builders.
        /// </summary>
        /// <param name="key">The globally unique key for the extension.</param>
        /// <param name="extension">The extension to add.</param>
        /// <returns>Whether the extension was able to be added.</returns>
        bool TryAddExtension(string key, object extension);

        /// <summary>
        /// Tries to get an extension.
        /// </summary>
        /// <param name="key">The globally unique key for the extension.</param>
        /// <returns>Whether the extension was able to be retrieved.</returns>
        object? TryGetExtension(string key);
    }
}

// ==============================
// Components
// ==============================
namespace TehPers.Core.Gui.Api.Components
{
    /// <summary>
    /// A size for use in a GUI.
    /// </summary>
    public interface IGuiSize
    {
        /// <summary>
        /// The width of the component.
        /// </summary>
        float Width { get; }

        /// <summary>
        /// The height of the component.
        /// </summary>
        float Height { get; }

        /// <summary>
        /// Deconstructs this <see cref="IGuiSize"/>.
        /// </summary>
        /// <param name="width">The width of the component.</param>
        /// <param name="height">The height of the component.</param>
        void Deconstruct(out float width, out float height);
    }

    /// <inheritdoc />
    /// <param name="Width">The width of the component.</param>
    /// <param name="Height">The height of the component.</param>
    public record GuiSize(float Width, float Height) : IGuiSize
    {
        /// <summary>
        /// A size of zero in both axes.
        /// </summary>
        public static GuiSize Zero => new(0, 0);

        /// <summary>
        /// A size of one in both axes.
        /// </summary>
        public static GuiSize One => new(1, 1);

        /// <summary>
        /// Converts a <see cref="Vector2"/> to a <see cref="GuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public GuiSize(Vector2 size)
            : this(size.X, size.Y)
        {
        }

        /// <summary>
        /// Converts a <see cref="Point"/> to a <see cref="GuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public GuiSize(Point size)
            : this(size.X, size.Y)
        {
        }
    }

    /// <summary>
    /// A partially defined size for use in a GUI.
    /// </summary>
    public interface IPartialGuiSize
    {
        /// <summary>
        /// The width of the component, if any.
        /// </summary>
        float? Width { get; }

        /// <summary>
        /// The height of the component, if any.
        /// </summary>
        float? Height { get; }

        /// <summary>
        /// Deconstructs this <see cref="IPartialGuiSize"/>.
        /// </summary>
        /// <param name="width">The width of the component, if any.</param>
        /// <param name="height">The height of the component, if any.</param>
        void Deconstruct(out float? width, out float? height);
    }

    /// <inheritdoc />
    /// <param name="Width">The width of the component, if any.</param>
    /// <param name="Height">The height of the component, if any.</param>
    public record PartialGuiSize(float? Width, float? Height) : IPartialGuiSize
    {
        /// <summary>
        /// An empty partial size. The size in each axis is <see langword="null"/>.
        /// </summary>
        public static PartialGuiSize Empty => new(null, null);

        /// <summary>
        /// A size of zero in both axes.
        /// </summary>
        public static PartialGuiSize Zero => new(0, 0);

        /// <summary>
        /// A size of one in both axes.
        /// </summary>
        public static PartialGuiSize One => new(1, 1);

        /// <summary>
        /// Converts a <see cref="GuiSize"/> to a <see cref="PartialGuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public PartialGuiSize(IGuiSize size)
            : this(size.Width, size.Height)
        {
        }

        /// <summary>
        /// Converts a <see cref="Vector2"/> to a <see cref="PartialGuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public PartialGuiSize(Vector2 size)
            : this(size.X, size.Y)
        {
        }

        /// <summary>
        /// Converts a <see cref="Point"/> to a <see cref="PartialGuiSize"/>.
        /// </summary>
        /// <param name="size">The source.</param>
        public PartialGuiSize(Point size)
            : this(size.X, size.Y)
        {
        }
    }

    /// <summary>
    /// Constraints on how an <see cref="IGuiComponent"/> should be rendered.
    /// </summary>
    public interface IGuiConstraints
    {
        /// <summary>
        /// The minimum size of this component. The component may be given an area with
        /// less size than this when being drawn, but it may not be rendered correctly if so. For
        /// example, it might get cut off or overlap into another component.
        /// </summary>
        public IGuiSize MinSize { get; }

        /// <summary>
        /// The maximum size of this component, if any. The component may be given an area with
        /// more size than this when being drawn, but it may not be rendered correctly if so. For
        /// example, there might be unexpected extra space around it or it might be stretched.
        /// </summary>
        public IPartialGuiSize MaxSize { get; }

        /// <summary>
        /// Deconstructs the constraints into its components.
        /// </summary>
        /// <param name="minSize">The minimum size constraint.</param>
        /// <param name="maxSize">The maximum size constraint.</param>
        public void Deconstruct(out IGuiSize minSize, out IPartialGuiSize maxSize)
        {
            minSize = this.MinSize;
            maxSize = this.MaxSize;
        }
    }

    /// <inheritdoc />
    /// <param name="MinSize">The minimum size of this component.</param>
    /// <param name="MaxSize">The maximum size of this component, if any.</param>
    public record GuiConstraints(
        IGuiSize? MinSize = null,
        IPartialGuiSize? MaxSize = null
    ) : IGuiConstraints
    {
        /// <inheritdoc />
        public IGuiSize MinSize { get; init; } = MinSize ?? GuiSize.Zero;

        /// <inheritdoc />
        public IPartialGuiSize MaxSize { get; init; } = MaxSize ?? PartialGuiSize.Empty;

        /// <summary>
        /// Deconstructs the constraints into its components.
        /// </summary>
        /// <param name="minSize">The minimum size constraint.</param>
        /// <param name="maxSize">The maximum size constraint.</param>
        public void Deconstruct(out IGuiSize minSize, out IPartialGuiSize maxSize)
        {
            minSize = this.MinSize;
            maxSize = this.MaxSize;
        }
    }

    /// <summary>
    /// A type of click.
    /// </summary>
    public enum ClickType
    {
        /// <summary>
        /// A click from the left mouse button.
        /// </summary>
        Left,

        /// <summary>
        /// A click from the middle mouse button.
        /// </summary>
        Middle,

        /// <summary>
        /// A click from the right mouse button.
        /// </summary>
        Right,
    }

    /// <summary>
    /// An event that can update the state of the GUI.
    /// </summary>
    public interface IGuiEvent
    {
        /// <summary>
        /// Checks whether this was caused by a regular update tick.
        /// </summary>
        /// <param name="time">The current game time.</param>
        bool IsUpdateTick([MaybeNullWhen(false)] out GameTime time)
        {
            time = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by a position being hovered over.
        /// </summary>
        /// <param name="position">The position being hovered over.</param>
        bool IsHover(out Point position)
        {
            position = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by a mouse click.
        /// </summary>
        /// <param name="position">The cursor position when receiving the click.</param>
        /// <param name="clickType">The type of click received.</param>
        bool IsReceiveClick(out Point position, out ClickType clickType)
        {
            position = default;
            clickType = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by the mouse being scrolled.
        /// </summary>
        /// <param name="direction">The direction and amount the mouse wheel was scrolled.</param>
        bool IsScroll(out int direction)
        {
            direction = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by text being input by the user.
        /// </summary>
        /// <param name="text">The text that was input.</param>
        bool IsTextInput([MaybeNullWhen(false)] out string text)
        {
            text = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by a keyboard key press.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        bool IsKeyboardInput(out Keys key)
        {
            key = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by a game pad button press.
        /// </summary>
        /// <param name="button">The button that was pressed.</param>
        bool IsGamePadInput(out Buttons button)
        {
            button = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by the component being drawn.
        /// </summary>
        /// <param name="batch">The sprite batch to draw with.</param>
        bool IsDraw([MaybeNullWhen(false)] out SpriteBatch batch)
        {
            batch = default;
            return false;
        }

        /// <summary>
        /// Checks whether this was caused by a custom event.
        /// </summary>
        /// <param name="data">The event data.</param>
        bool IsOther([MaybeNullWhen(false)] out object data)
        {
            data = default;
            return false;
        }
    }

    /// <inheritdoc />
    public abstract record GuiEvent : IGuiEvent
    {
        private GuiEvent() { }

        /// <summary>
        /// A regular update tick.
        /// </summary>
        /// <param name="Time">The current game time.</param>
        public sealed record UpdateTick(GameTime Time) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsUpdateTick(out GameTime time)
            {
                time = this.Time;
                return true;
            }
        }

        /// <summary>
        /// A position was hovered over by the cursor.
        /// </summary>
        /// <param name="Position">The position being hovered over.</param>
        public sealed record Hover(Point Position) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsHover(out Point position)
            {
                position = this.Position;
                return true;
            }
        }

        /// <summary>
        /// A mouse click was received.
        /// </summary>
        /// <param name="Position">The cursor position when receiving the click.</param>
        /// <param name="ClickType">The type of click received.</param>
        public sealed record ReceiveClick(Point Position, ClickType ClickType) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsReceiveClick(out Point position, out ClickType clickType)
            {
                position = this.Position;
                clickType = this.ClickType;
                return true;
            }
        }

        /// <summary>
        /// The mouse was scrolled.
        /// </summary>
        /// <param name="Direction">The direction and amount the mouse wheel was scrolled.</param>
        public sealed record Scroll(int Direction) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsScroll(out int direction)
            {
                direction = this.Direction;
                return true;
            }
        }

        /// <summary>
        /// Text was input by the user.
        /// </summary>
        /// <param name="Text">The text that was input.</param>
        public sealed record TextInput(string Text) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsTextInput(out string text)
            {
                text = this.Text;
                return true;
            }
        }

        /// <summary>
        /// A keyboard key press was received.
        /// </summary>
        /// <param name="Key">The key that was pressed.</param>
        public sealed record KeyboardInput(Keys Key) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsKeyboardInput(out Keys key)
            {
                key = this.Key;
                return true;
            }
        }

        /// <summary>
        /// A game pad button press was received.
        /// </summary>
        /// <param name="Button">The button that was pressed.</param>
        public sealed record GamePadInput(Buttons Button) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsGamePadInput(out Buttons button)
            {
                button = this.Button;
                return true;
            }
        }

        /// <summary>
        /// The component is being drawn.
        /// </summary>
        /// <param name="Batch">The sprite batch to draw with.</param>
        public sealed record Draw(SpriteBatch Batch) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsDraw(out SpriteBatch batch)
            {
                batch = this.Batch;
                return true;
            }
        }

        /// <summary>
        /// A custom event.
        /// </summary>
        /// <param name="Data">The event data.</param>
        public sealed record Other(object Data) : GuiEvent, IGuiEvent
        {
            /// <inheritdoc />
            public bool IsOther(out object data)
            {
                data = this.Data;
                return true;
            }
        }
    }

    /// <summary>
    /// A GUI component that can be drawn to the screen.
    /// </summary>
    public interface IGuiComponent
    {
        /// <summary>
        /// Gets the GUI builder.
        /// </summary>
        public IGuiBuilder GuiBuilder { get; }

        /// <summary>
        /// Gets the constraints on how this component should be rendered.
        /// </summary>
        /// <returns>The constraints on how this component should be rendered.</returns>
        IGuiConstraints GetConstraints();

        /// <summary>
        /// Handles a UI event. This is where the component draws to the screen, handles input, and
        /// does whatever else it needs to do.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <returns>Whether this component was updated.</returns>
        void Handle(IGuiEvent e, Rectangle bounds);
    }

    /// <summary>
    /// A component with a configurable layer depth.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    public interface IWithLayerDepth<out TComponent>
    {
        /// <summary>
        /// Sets the layer depth this component is rendered at.
        /// </summary>
        /// <param name="layerDepth">The new layer depth to render the component at.</param>
        /// <returns>The resulting component.</returns>
        TComponent WithLayerDepth(float layerDepth);
    }

    /// <summary>
    /// A component which contains an inner component.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    public interface IWithInner<out TComponent>
    {
        /// <summary>
        /// Sets the inner component.
        /// </summary>
        /// <param name="inner">The new inner component.</param>
        /// <returns>The resulting component.</returns>
        TComponent WithInner(IGuiComponent inner);
    }

    /// <summary>
    /// A component which contains state.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TComponent">The component type.</typeparam>
    public interface IWithState<in TState, out TComponent>
    {
        /// <summary>
        /// Sets the state of this component.
        /// </summary>
        /// <param name="state">The new state of this component.</param>
        /// <returns>The resulting component.</returns>
        TComponent WithState(TState state);
    }

    /// <summary>
    /// Aligns a component. This removes any maximum size constraints on aligned axes.
    /// </summary>
    public interface IAligner : IGuiComponent, IWithInner<IAligner>
    {
        /// <summary>
        /// Sets the horizontal alignment of this component.
        /// </summary>
        /// <param name="alignment">The new horizontal alignment, if any.</param>
        /// <returns>The resulting component.</returns>
        IAligner WithHorizontalAlignment(HorizontalAlignment alignment);

        /// <summary>
        /// Sets the vertical alignment of this component.
        /// </summary>
        /// <param name="alignment">The new vertical alignment, if any.</param>
        /// <returns>The resulting component.</returns>
        IAligner WithVerticalAlignment(VerticalAlignment alignment);
    }

    /// <summary>
    /// Alignment along the x-axis.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// No horizontal alignment should be done.
        /// </summary>
        None,

        /// <summary>
        /// The left side of the contents are aligned to the left side of its container.
        /// </summary>
        Left,

        /// <summary>
        /// The center of the contents are aligned horizontally to the center of its container.
        /// </summary>
        Center,

        /// <summary>
        /// The right side of the contents are aligned horizontally to the center of its container.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Alignment along the y-axis.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// No vertical alignment should be done.
        /// </summary>
        None,

        /// <summary>
        /// The top of the contents are aligned to the top of its container.
        /// </summary>
        Top,

        /// <summary>
        /// The center of the contents are aligned vertically to the center of its container.
        /// </summary>
        Center,

        /// <summary>
        /// The bottom of the contents are aligned to the bottom of its container.
        /// </summary>
        Bottom,
    }

    /// <summary>
    /// A component with a background.
    /// </summary>
    public interface IBackground : IGuiComponent, IWithInner<IBackground>
    {
        /// <summary>
        /// Sets the background component.
        /// </summary>
        /// <param name="background">The new background component.</param>
        /// <returns>The resulting component.</returns>
        IBackground WithBackground(IGuiComponent background);
    }

    /// <summary>
    /// A checkbox component.
    /// </summary>
    public interface ICheckbox : IGuiComponent, IWithState<ICheckbox.IState, ICheckbox>
    {
        /// <summary>
        /// The state of a checkbox.
        /// </summary>
        public interface IState
        {
            /// <summary>
            /// Gets or sets whether the checkbox is checked.
            /// </summary>
            public bool Checked { get; set; }
        }

        /// <inheritdoc />
        public class State : IState
        {
            /// <inheritdoc />
            public bool Checked { get; set; }
        }
    }

    /// <summary>
    /// A component which detects a click.
    /// </summary>
    public interface IClickDetector : IGuiComponent, IWithInner<IClickDetector>
    {
        /// <summary>
        /// Sets the action to execute when this component is clicked.
        /// </summary>
        /// <param name="action">The new action to execute.</param>
        /// <returns>The resulting component.</returns>
        IClickDetector WithAction(Action<ClickType> action);
    }

    /// <summary>
    /// Clips this component, removing its minimum size constraint. This constrains its rendering
    /// area and mouse inputs if it is shrunk.
    /// </summary>
    public interface IClipper : IGuiComponent, IWithInner<IClipper>
    {
    }

    /// <summary>
    /// Adds padding to a component.
    /// </summary>
    public interface IComponentPadder : IGuiComponent, IWithInner<IComponentPadder>
    {
        /// <summary>
        /// Sets the left padding of this component.
        /// </summary>
        /// <param name="padding">The new padding.</param>
        /// <returns>The resulting component.</returns>
        IComponentPadder WithLeft(float padding);

        /// <summary>
        /// Sets the right padding of this component.
        /// </summary>
        /// <param name="padding">The new padding.</param>
        /// <returns>The resulting component.</returns>
        IComponentPadder WithRight(float padding);

        /// <summary>
        /// Sets the top padding of this component.
        /// </summary>
        /// <param name="padding">The new padding.</param>
        /// <returns>The resulting component.</returns>
        IComponentPadder WithTop(float padding);

        /// <summary>
        /// Sets the bottom padding of this component.
        /// </summary>
        /// <param name="padding">The new padding.</param>
        /// <returns>The resulting component.</returns>
        IComponentPadder WithBottom(float padding);
    }

    /// <summary>
    /// Adds additional constraints to a component's size.
    /// </summary>
    public interface IConstrainer : IGuiComponent, IWithInner<IConstrainer>
    {
        /// <summary>
        /// Sets the additional minimum size constraints for the component. This cannot make the
        /// component smaller than its previous minimum size.
        /// </summary>
        /// <param name="minSize">The new minimum size (if the inner component will fit).</param>
        /// <returns>The resulting component.</returns>
        IConstrainer WithMinSize(IPartialGuiSize minSize);

        /// <summary>
        /// Sets the additional maximum size constraints for the component. This cannot make the
        /// component larger than its previous maximum size.
        /// </summary>
        /// <param name="maxSize">The new maximum size (if the inner component will fit).</param>
        /// <returns>The resulting component.</returns>
        IConstrainer WithMaxSize(IPartialGuiSize maxSize);
    }

    /// <summary>
    /// A dialogue box.
    /// </summary>
    public interface IDialogueBox : IGuiComponent
    {
        /// <summary>
        /// Sets the portait to show for the speaker, if any.
        /// </summary>
        /// <param name="portrait">The new portrait, if any.</param>
        /// <returns>The resulting component.</returns>
        IDialogueBox WithSpeakerPortrait(Portrait portrait);

        /// <summary>
        /// Sets whether to draw only the dialogue box itself.
        /// </summary>
        /// <param name="drawOnlyBox">Whether to draw only the dialogue box.</param>
        /// <returns>The resulting component.</returns>
        IDialogueBox WithDrawOnlyBox(bool drawOnlyBox);

        /// <summary>
        /// Sets the message to show in the dialogue box, if any.
        /// </summary>
        /// <param name="message">The new message, if any.</param>
        /// <returns>The resulting component.</returns>
        IDialogueBox WithMessage(string? message);

        /// <summary>
        /// The speaker portrait to show in a dialogue box, if any.
        /// </summary>
        public enum Portrait
        {
            /// <summary>
            /// No portrait should be shown.
            /// </summary>
            None,

            /// <summary>
            /// The portrait for <see cref="Game1.currentSpeaker"/> should be shown.
            /// </summary>
            CurrentSpeakerPortrait,

            /// <summary>
            /// The portrait for <see cref="Game1.objectDialoguePortraitPerson"/> should be shown.
            /// </summary>
            ObjectPortrait,
        }
    }

    /// <summary>
    /// A dropdown choice selector.
    /// </summary>
    /// <typeparam name="T">The type of item the dropdown holds.</typeparam>
    public interface IDropdown<T> : IGuiComponent, IWithState<IDropdown<T>.IState, IDropdown<T>>,
        IWithLayerDepth<IDropdown<T>>
    {
        /// <summary>
        /// The state of a dropdown.
        /// </summary>
        public interface IState
        {
            /// <summary>
            /// The maximum number of items that should be visible at a time.
            /// </summary>
            int MaxVisibleItems { get; set; }

            /// <summary>
            /// The index of the top item that is currently visible.
            /// </summary>
            int TopVisibleIndex { get; set; }

            /// <summary>
            /// The index of the hovered item.
            /// </summary>
            int? HoveredIndex { get; set; }

            /// <summary>
            /// Gets or sets whether the dropdown is dropped.
            /// </summary>
            bool Dropped { get; set; }

            /// <summary>
            /// Gets or sets the items in the dropdown.
            /// </summary>
            List<(T Item, string Label)> Items { get; set; }

            /// <summary>
            /// Gets or sets the index of the selected item.
            /// </summary>
            int SelectedIndex { get; set; }

            /// <summary>
            /// The selected item, if any.
            /// </summary>
            (T Item, string Label)? Selected =>
                this.Items.Any() ? this.Items[this.SelectedIndex] : null;
        }

        /// <inheritdoc />
        public class State : IState
        {
            private int index;
            private int maxVisibleItems = 4;
            private int topVisibleIndex;
            private int? hoveredIndex;

            /// <inheritdoc />
            public bool Dropped { get; set; }

            /// <inheritdoc />
            public List<(T Item, string Label)> Items { get; set; }

            /// <inheritdoc />
            public int SelectedIndex
            {
                get => this.Items.Any() ? Math.Clamp(this.index, 0, this.Items.Count - 1) : 0;
                set => this.index = value;
            }

            /// <inheritdoc />
            public int MaxVisibleItems
            {
                get => this.maxVisibleItems;
                set => this.maxVisibleItems = Math.Min(1, value);
            }

            /// <inheritdoc />
            public int TopVisibleIndex
            {
                get => this.Items.Count > this.MaxVisibleItems
                    ? Math.Clamp(this.topVisibleIndex, 0, this.Items.Count - this.MaxVisibleItems)
                    : 0;
                set => this.topVisibleIndex = value;
            }

            /// <inheritdoc />
            public int? HoveredIndex
            {
                get => this.hoveredIndex is { } hoveredIndex && this.Dropped && this.Items.Any()
                    ? Math.Clamp(hoveredIndex, 0, this.Items.Count - 1)
                    : null;
                set => this.hoveredIndex = value;
            }

            /// <summary>
            /// Creates a new dropdown state.
            /// </summary>
            /// <param name="items">The items in the dropdown menu.</param>
            public State(List<(T Item, string Label)> items)
            {
                this.Items = items;
            }
        }
    }

    /// <summary>
    /// A component that executes an action when hovered.
    /// </summary>
    public interface IHoverDetector : IGuiComponent, IWithInner<IHoverDetector>
    {
        /// <summary>
        /// Sets the action to execute when this component is hovered.
        /// </summary>
        /// <param name="action">The new action to execute.</param>
        /// <returns>The resulting component.</returns>
        IHoverDetector WithAction(Action action);
    }

    /// <summary>
    /// A component which renders an item.
    /// </summary>
    public interface IItemView : IGuiComponent, IWithLayerDepth<IItemView>
    {
        /// <summary>
        /// Sets the item to show in this view.
        /// </summary>
        /// <param name="item">The item to show.</param>
        /// <returns>The resulting component.</returns>
        IItemView WithItem(Item item);

        /// <summary>
        /// Sets the transparency of the item.
        /// </summary>
        /// <param name="transparency">The transparency of the item.</param>
        /// <returns>The resulting component.</returns>
        IItemView WithTransparency(float transparency);

        /// <summary>
        /// Sets the side length of this item view.
        /// </summary>
        /// <param name="sideLength">The side length.</param>
        /// <returns>The resulting component.</returns>
        IItemView WithSideLength(float sideLength);

        /// <summary>
        /// Sets how to draw the stack number, if any.
        /// </summary>
        /// <param name="drawType">How the stack number should be drawn.</param>
        /// <returns>The resulting component.</returns>
        IItemView WithStackDrawType(StackDrawType drawType);

        /// <summary>
        /// Sets the color to tint the item.
        /// </summary>
        /// <param name="color">The tint color.</param>
        /// <returns>The resulting component.</returns>
        IItemView WithColor(Color color);

        /// <summary>
        /// Sets whether to draw the item's shadow.
        /// </summary>
        /// <param name="drawShadow">Whether to draw the shadow.</param>
        /// <returns>The resulting component.</returns>
        IItemView WithShadow(bool drawShadow);
    }

    /// <summary>
    /// A UI label.
    /// </summary>
    public interface ILabel : IGuiComponent, IWithLayerDepth<ILabel>
    {
        /// <summary>
        /// Sets the text of this label.
        /// </summary>
        /// <param name="text">The new text for the label.</param>
        /// <returns>The resulting component.</returns>
        ILabel WithText(string text);

        /// <summary>
        /// Sets the font this label is rendered with.
        /// </summary>
        /// <param name="font">The new font for the label.</param>
        /// <returns>The resulting component.</returns>
        ILabel WithFont(SpriteFont font);

        /// <summary>
        /// Sets the text color of this label.
        /// </summary>
        /// <param name="color">The new text color for the label.</param>
        /// <returns>The resulting component.</returns>
        ILabel WithColor(Color color);

        /// <summary>
        /// Sets the text scale of this label.
        /// </summary>
        /// <param name="scale">The new text scale for the label.</param>
        /// <returns>The resulting component.</returns>
        ILabel WithScale(Vector2 scale);

        /// <summary>
        /// Sets the text scale of this label.
        /// </summary>
        /// <param name="scale">The new text scale for the label (along both x and y).</param>
        /// <returns>The resulting component.</returns>
        ILabel WithScale(float scale)
        {
            return this.WithScale(new Vector2(scale, scale));
        }

        /// <summary>
        /// Sets the sprite effect of this label.
        /// </summary>
        /// <param name="spriteEffects">The new sprite effects for the label.</param>
        /// <returns>The resulting component.</returns>
        ILabel WithSpriteEffects(SpriteEffects spriteEffects);
    }

    /// <summary>
    /// A vertical scrollbar.
    /// </summary>
    public interface IVerticalScrollbar : IGuiComponent, IWithLayerDepth<IVerticalScrollbar>,
        IWithState<IVerticalScrollbar.IState, IVerticalScrollbar>
    {
        /// <summary>
        /// The state of a scrollbar.
        /// </summary>
        public interface IState
        {
            /// <summary>
            /// Gets or sets the current value this scrollbar holds.
            /// </summary>
            int Value { get; set; }

            /// <summary>
            /// Gets or sets the inclusive minimum value of this scrollbar.
            /// </summary>
            int MinValue { get; set; }

            /// <summary>
            /// Gets or sets the inclusive maximum value of this scrollbar.
            /// </summary>
            int MaxValue { get; set; }

            /// <summary>
            /// Gets the percentage the current value is between the minimum and maximum value.
            /// </summary>
            float Percentage =>
                (float)(this.Value - this.MinValue) / (this.MaxValue - this.MinValue);
        }

        /// <inheritdoc />
        public class State : IState
        {
            private int value;

            /// <inheritdoc />
            public int MinValue { get; set; }

            /// <inheritdoc />
            public int MaxValue { get; set; }

            /// <inheritdoc />
            public int Value
            {
                get => Math.Clamp(this.value, this.MinValue, this.MaxValue);
                set => this.value = value;
            }
        }
    }

    /// <summary>
    /// Shrinks a GUI component to its minimum size.
    /// </summary>
    public interface IShrinker : IGuiComponent, IWithInner<IShrinker>
    {
    }

    /// <summary>
    /// A component with simple functionality.
    /// </summary>
    public interface ISimpleComponent : IGuiComponent
    {
        /// <summary>
        /// Sets the constraints for this component.
        /// </summary>
        /// <param name="constraints">The new constraints.</param>
        /// <returns>The resulting component.</returns>
        ISimpleComponent WithConstraints(IGuiConstraints constraints);

        /// <summary>
        /// Sets the action to execute when drawing this component.
        /// </summary>
        /// <param name="draw">The new action to execute when drawing this component.</param>
        /// <returns>The resulting component.</returns>
        ISimpleComponent WithDrawAction(Action<SpriteBatch, Rectangle> draw);
    }

    /// <summary>
    /// A texture that is stretched to fill a space.
    /// </summary>
    public interface IStretchedTexture : IGuiComponent, IWithLayerDepth<IStretchedTexture>
    {
        /// <summary>
        /// Sets the texture to draw.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <returns>The resulting component.</returns>
        IStretchedTexture WithTexture(Texture2D texture);

        /// <summary>
        /// Sets the source rectangle on the texture.
        /// </summary>
        /// <param name="sourceRectangle"></param>
        /// <returns>The resulting component.</returns>
        IStretchedTexture WithSourceRectangle(Rectangle? sourceRectangle);

        /// <summary>
        /// Sets the color to tint the texture.
        /// </summary>
        /// <param name="color">The color to tint the texture.</param>
        /// <returns>The resulting component.</returns>
        IStretchedTexture WithColor(Color color);

        /// <summary>
        /// Sets the sprite effects to apply to the texture.
        /// </summary>
        /// <param name="effects">The sprite effects to apply to the texture.</param>
        /// <returns>The resulting component.</returns>
        IStretchedTexture WithSpriteEffects(SpriteEffects effects);

        /// <summary>
        /// Sets the minimum scaled size of this texture. A min scaled width of 2 means that this
        /// texture must be stretched by at least double its original width, for example.
        /// </summary>
        /// <param name="minScale">The minimum scaled size of this texture.</param>
        /// <returns>The resulting component.</returns>
        IStretchedTexture WithMinScale(IGuiSize minScale);

        /// <summary>
        /// Sets the maximum scaled size of this texture. A max scaled width of 2 means that this
        /// texture can only be stretched up to double its original width, for example.
        /// </summary>
        /// <param name="maxScale">The maximum scaled size of this texture.</param>
        /// <returns>The resulting component.</returns>
        IStretchedTexture WithMaxScale(IPartialGuiSize maxScale);
    }

    /// <summary>
    /// A standard text box.
    /// </summary>
    public interface ITextBox : IGuiComponent, IWithState<ITextInput.IState, ITextBox>
    {
        /// <summary>
        /// Sets the input helper to use for tracking input.
        /// </summary>
        /// <param name="inputHelper">The input helper.</param>
        /// <returns>The resulting component</returns>
        ITextBox WithInputHelper(IInputHelper inputHelper);
    }

    /// <summary>
    /// A single-line text input component.
    /// </summary>
    public interface ITextInput : IGuiComponent, IWithLayerDepth<ITextInput>,
        IWithState<ITextInput.IState, ITextInput>
    {
        /// <summary>
        /// Sets the input helper to use for tracking input.
        /// </summary>
        /// <param name="inputHelper">The input helper.</param>
        /// <returns>The resulting component</returns>
        ITextInput WithInputHelper(IInputHelper inputHelper);

        /// <summary>
        /// Sets the text font.
        /// </summary>
        /// <param name="font">The new text font.</param>
        /// <returns>The resulting component.</returns>
        ITextInput WithFont(SpriteFont font);

        /// <summary>
        /// Sets the text color.
        /// </summary>
        /// <param name="color">The new text color.</param>
        /// <returns>The resulting component.</returns>
        ITextInput WithTextColor(Color color);

        /// <summary>
        /// Sets the cursor color.
        /// </summary>
        /// <param name="color">The new cursor color.</param>
        /// <returns>The resulting component.</returns>
        ITextInput WithCursorColor(Color? color);

        /// <summary>
        /// Sets the highlighted text color.
        /// </summary>
        /// <param name="color">The new highlighted text color.</param>
        /// <returns>The resulting component.</returns>
        ITextInput WithHighlightedTextColor(Color? color);

        /// <summary>
        /// Sets the highlighted text background color.
        /// </summary>
        /// <param name="color">The new highighted text background color.</param>
        /// <returns>The resulting component.</returns>
        ITextInput WithHighlightedTextBackgroundColor(Color? color);

        /// <summary>
        /// Sets whether the text input should be unfocused when the ESC key is pressed.
        /// </summary>
        /// <param name="unfocus">Whether the text input should be unfocused.</param>
        /// <returns>The resulting component.</returns>
        ITextInput WithUnfocusOnEsc(bool unfocus);

        /// <summary>
        /// Sets the default cue that is played when text is inserted.
        /// </summary>
        /// <param name="insertionCue">The new default cue to play.</param>
        /// <returns>Ther esulting component.</returns>
        ITextInput WithDefaultInsertionCue(string? insertionCue);

        /// <summary>
        /// Sets the cues to play when specific characters are typed.
        /// </summary>
        /// <param name="insertionCues"></param>
        /// <returns></returns>
        ITextInput WithInsertionCues(IDictionary<char, string?> insertionCues);

        /// <summary>
        /// The state for a text input.
        /// </summary>
        public interface IState
        {
            /// <summary>
            /// The text in the input.
            /// </summary>
            string Text { get; set; }

            /// <summary>
            /// The anchor cursor. This is the primary one that's moved around.
            /// </summary>
            int AnchorCursor { get; set; }

            /// <summary>
            /// The selection cursor, if any text is selected. This is the one used whenever text
            /// is being selected, and may come before or after the anchor cursor.
            /// </summary>
            int? SelectionCursor { get; set; }

            /// <summary>
            /// Whether the input is focused.
            /// </summary>
            bool Focused { get; set; }

            /// <summary>
            /// The method of text insertion.
            /// </summary>
            InsertMode InsertionMode { get; set; }

            /// <summary>
            /// Gets the range of selected text, if any.
            /// </summary>
            (int Start, int End)? Selection
            {
                get
                {
                    if (this.SelectionCursor is not { } selectionCursor)
                    {
                        return null;
                    }

                    return selectionCursor > this.AnchorCursor
                        ? (this.AnchorCursor, selectionCursor)
                        : (selectionCursor, this.AnchorCursor);
                }
            }
        }

        /// <inheritdoc />
        public class State : IState
        {
            private string text = string.Empty;
            private int anchorCursor;
            private int? selectionCursor;

            /// <inheritdoc />
            public string Text
            {
                get => this.text;
                set
                {
                    this.text = value;
                    if (this.anchorCursor > this.text.Length)
                    {
                        this.anchorCursor = this.text.Length;
                    }

                    if (this.selectionCursor is { } selectionCursor
                        && selectionCursor > this.text.Length)
                    {
                        this.SelectionCursor = this.text.Length;
                    }
                }
            }

            /// <inheritdoc />
            public bool Focused { get; set; }

            /// <inheritdoc />
            public InsertMode InsertionMode { get; set; } = InsertMode.Insert;

            /// <inheritdoc />
            public int AnchorCursor
            {
                get => this.anchorCursor;
                set
                {
                    if (this.selectionCursor == value)
                    {
                        this.selectionCursor = null;
                    }

                    this.anchorCursor = Math.Clamp(value, 0, this.Text.Length);
                }
            }

            /// <inheritdoc />
            public int? SelectionCursor
            {
                get => this.selectionCursor;
                set
                {
                    if (this.anchorCursor == value)
                    {
                        this.selectionCursor = null;
                        return;
                    }

                    this.selectionCursor =
                        value is { } val ? Math.Clamp(val, 0, this.Text.Length) : null;
                }
            }

            /// <inheritdoc />
            public (int Start, int End)? Selection
            {
                get
                {
                    if (this.SelectionCursor is not { } selectionCursor)
                    {
                        return null;
                    }

                    return selectionCursor > this.AnchorCursor
                        ? (this.AnchorCursor, selectionCursor)
                        : (selectionCursor, this.AnchorCursor);
                }
            }
        }

        /// <summary>
        /// Modes of text insertion.
        /// </summary>
        public enum InsertMode
        {
            /// <summary>
            /// Inserts the text at the cursor position.
            /// </summary>
            Insert,

            /// <summary>
            /// Replaces the text at the cursor position.
            /// </summary>
            Replace,
        }
    }

    /// <summary>
    /// A component which fills a space with a texture created from a 3x3 grid.
    /// </summary>
    public interface ITextureBox : IGuiComponent, IWithLayerDepth<ITextureBox>
    {
        /// <summary>
        /// Sets the source texture to draw from.
        /// </summary>
        /// <param name="texture">The new source texture.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithTexture(Texture2D texture);

        /// <summary>
        /// Sets the source rectangle for the top left.
        /// </summary>
        /// <param name="topLeft">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithTopLeft(Rectangle? topLeft);

        /// <summary>
        /// Sets the source rectangle for the top center.
        /// </summary>
        /// <param name="topCenter">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithTopCenter(Rectangle? topCenter);

        /// <summary>
        /// Sets the source rectangle for the top right.
        /// </summary>
        /// <param name="topRight">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithTopRight(Rectangle? topRight);

        /// <summary>
        /// Sets the source rectangle for the center left.
        /// </summary>
        /// <param name="centerLeft">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithCenterLeft(Rectangle? centerLeft);

        /// <summary>
        /// Sets the source rectangle for the center.
        /// </summary>
        /// <param name="center">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithCenter(Rectangle? center);

        /// <summary>
        /// Sets the source rectangle for the center right.
        /// </summary>
        /// <param name="centerRight">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithCenterRight(Rectangle? centerRight);

        /// <summary>
        /// Sets the source rectangle for the bottom left.
        /// </summary>
        /// <param name="bottomLeft">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithBottomLeft(Rectangle? bottomLeft);

        /// <summary>
        /// Sets the source rectangle for the bottom center.
        /// </summary>
        /// <param name="bottomCenter">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithBottomCenter(Rectangle? bottomCenter);

        /// <summary>
        /// Sets the source rectangle for the bottom right.
        /// </summary>
        /// <param name="bottomRight">The new source rectangle.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithBottomRight(Rectangle? bottomRight);

        /// <summary>
        /// Sets the minimum scale of the individual texture parts.
        /// </summary>
        /// <param name="minScale">The new minimum scale.</param>
        /// <returns>The resulting component.</returns>
        ITextureBox WithMinScale(IGuiSize minScale);
    }

    /// <summary>
    /// A vertically scrollable view that clips its inner component.
    /// </summary>
    public interface IVerticalScrollArea : IGuiComponent, IWithInner<IVerticalScrollArea>,
        IWithState<IVerticalScrollbar.IState, IVerticalScrollArea>
    {
    }
}

// ==============================
// Layouts
// ==============================
namespace TehPers.Core.Gui.Api.Components.Layouts
{
    /// <summary>
    /// A builder for a GUI layout.
    /// </summary>
    public interface ILayoutBuilder
    {
        /// <summary>
        /// Adds a new component to this layout.
        /// </summary>
        /// <param name="component">The component to add to the layout.</param>
        void Add(IGuiComponent component);
    }

    /// <summary>
    /// A GUI layout.
    /// </summary>
    /// <typeparam name="TLayout">The interface for the concrete layout type.</typeparam>
    public interface ILayout<out TLayout> : IGuiComponent
    {
        /// <summary>
        /// Sets the components that are a part of this layout.
        /// </summary>
        /// <param name="components">The new components that are a part of this layout.</param>
        /// <returns>The resulting component.</returns>
        TLayout WithComponents(IEnumerable<IGuiComponent> components);

        /// <summary>
        /// Sets the components that are a part of this layout.
        /// </summary>
        /// <param name="components">The new components that are a part of this layout.</param>
        /// <returns>The resulting component.</returns>
        TLayout WithComponents(params IGuiComponent[] components)
        {
            return this.WithComponents(components.AsEnumerable());
        }
    }

    /// <summary>
    /// A horizontal layout container. Components are rendered horizontally along a single row.
    /// </summary>
    public interface IHorizontalLayout : ILayout<IHorizontalLayout>
    {
    }

    /// <summary>
    /// A vertical layout container. Components are rendered vertically along a single column.
    /// </summary>
    public interface IVerticalLayout : ILayout<IVerticalLayout>
    {
    }
}

// ==============================
// Component provider
// ==============================
namespace TehPers.Core.Gui.Api.Components
{
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
            HorizontalAlignment horizontal,
            VerticalAlignment vertical
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
        IComponentPadder Padder(
            IGuiComponent inner,
            float left,
            float right,
            float top,
            float bottom
        );

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
        IVerticalScrollArea VerticalScrollArea(
            IGuiComponent inner,
            IVerticalScrollbar.IState state
        );

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
}

// ==============================
// Internal use
// ==============================
namespace TehPers.Core.Gui.Api.Internal
{
    /// <summary>
    /// Internal use only APIs.
    /// </summary>
    public interface IInternalApi
    {
        /// <summary>
        /// Creates a default component provider.
        /// </summary>
        /// <returns>A new default component provider.</returns>
        IDefaultComponentProvider CreateDefaultComponentProvider();
    }
}

// ==============================
// Extension methods
// ==============================
namespace TehPers.Core.Gui.Api.Extensions
{
    /// <summary>
    /// Initialization methods for TehCore - Gui.
    /// </summary>
    public static class ModInitializer
    {
        internal const string modUniqueId = "TehPers.Core.Gui";

        internal static readonly string extensionUniqueId =
            $"{ModInitializer.modUniqueId}:defaultComponentProvider/{typeof(ModInitializer).Assembly.FullName}";

        /// <summary>
        /// Gets and initializes the TehCore GUI API.
        /// </summary>
        /// <param name="registry">The mod registry.</param>
        /// <returns>The TehCore GUI API.</returns>
        public static ICoreGuiApi GetGuiApi(this IModRegistry registry)
        {
            // Get the core mod API
            var api = registry.GetApi<ICoreGuiApi>(ModInitializer.modUniqueId);
            if (api is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ModInitializer.GetGuiApi)} must be called after TehCore - Gui loads. Make sure to add '{ModInitializer.modUniqueId}' as a dependency to your mod's manifest.json."
                );
            }

            // Initialize the default component provider
            // This must be done by each mod using TehPers.Core.Gui because the interfaces are different types (with the same names) for each consumer
            ModInitializer.InitializeGuiApi(api);

            return api;
        }

        /// <summary>
        /// Initializes the TehCore - GUI API. This is automatically called by <see cref="GetGuiApi"/>.
        /// </summary>
        /// <param name="api">The TehCore - GUI API.</param>
        public static void InitializeGuiApi(ICoreGuiApi api)
        {
            api.GuiBuilder.TryAddExtension(
                ModInitializer.extensionUniqueId,
                api.InternalApi.CreateDefaultComponentProvider()
            );
        }
    }

    /// <summary>
    /// Convenient extension methods for <see cref="GuiEvent"/>.
    /// </summary>
    public static class GuiEvents
    {
        /// <summary>
        /// Draws if the event is <see cref="GuiEvent.Draw"/>.
        /// </summary>
        /// <param name="e">The GUI event.</param>
        /// <param name="draw">A callback which draws.</param>
        public static void Draw(this IGuiEvent e, Action<SpriteBatch> draw)
        {
            if (e.IsDraw(out var batch))
            {
                draw(batch);
            }
        }

        /// <summary>
        /// Checks whether the given bounds were clicked.
        /// </summary>
        /// <param name="e">The GUI event.</param>
        /// <param name="bounds">The bounds to check for clicks within.</param>
        /// <param name="clickType">The type of click to check for.</param>
        /// <returns>Whether the given bounds were clicked.</returns>
        public static bool Clicked(this IGuiEvent e, Rectangle bounds, ClickType clickType)
        {
            return e.ClickType(bounds) == clickType;
        }

        /// <summary>
        /// Gets the type of click that was made within certain bounds.
        /// </summary>
        /// <param name="e">The GUI event.</param>
        /// <param name="bounds">The bounds to check for clicks within.</param>
        /// <returns>The type of click that was made, or <see langword="null"/> if no click was made within the bounds.</returns>
        public static ClickType? ClickType(this IGuiEvent e, Rectangle bounds)
        {
            return e.IsReceiveClick(out var position, out var type) && bounds.Contains(position)
                ? type
                : null;
        }
    }

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
            HorizontalAlignment horizontal = HorizontalAlignment.None,
            VerticalAlignment vertical = VerticalAlignment.None
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

    /// <summary>
    /// A T-connector on the end of a menu separator.
    /// </summary>
    public enum MenuSeparatorConnector
    {
        /// <summary>
        /// Connect to a menu border with a pin.
        /// </summary>
        PinMenuBorder,

        /// <summary>
        /// Smoothly connect to a menu border.
        /// </summary>
        MenuBorder,

        /// <summary>
        /// Connect to another separator.
        /// </summary>
        Separator,

        /// <summary>
        /// No connector.
        /// </summary>
        None,
    }

    /// <summary>
    /// Extension methods for working with <see cref="ILayoutBuilder"/>.
    /// </summary>
    public static class LayoutBuilder
    {
        /// <summary>
        /// Adds this component to a layout.
        /// </summary>
        /// <param name="component">The component to add to the layout.</param>
        /// <param name="builder">The layout builder.</param>
        public static void AddTo(this IGuiComponent component, ILayoutBuilder builder)
        {
            builder.Add(component);
        }

        /// <summary>
        /// Creates a new layout builder which aligns all the components added to it.
        /// </summary>
        /// <param name="builder">The inner layout builder.</param>
        /// <param name="horizontal">The horizontal alignment, if any.</param>
        /// <param name="vertical">The vertical alignment, if any.</param>
        /// <returns>The new layout builder.</returns>
        public static ILayoutBuilder Aligned(
            this ILayoutBuilder builder,
            HorizontalAlignment horizontal = HorizontalAlignment.None,
            VerticalAlignment vertical = VerticalAlignment.None
        )
        {
            return builder.Select(c => c.Aligned(horizontal, vertical));
        }

        /// <summary>
        /// Creates a new layout builder which applies a function to each component added to it.
        /// </summary>
        /// <param name="builder">The layout builder.</param>
        /// <param name="mapComponent">A function which maps each component to a new component.</param>
        /// <returns>The new layout builder.</returns>
        public static ILayoutBuilder Select(
            this ILayoutBuilder builder,
            Func<IGuiComponent, IGuiComponent> mapComponent
        )
        {
            return new MappedLayoutBuilder(builder, mapComponent);
        }

        private class MappedLayoutBuilder : ILayoutBuilder
        {
            private readonly ILayoutBuilder inner;
            private readonly Func<IGuiComponent, IGuiComponent> mapComponent;

            public MappedLayoutBuilder(
                ILayoutBuilder layoutBuilder,
                Func<IGuiComponent, IGuiComponent> mapComponent
            )
            {
                this.inner = layoutBuilder
                    ?? throw new ArgumentNullException(nameof(layoutBuilder));
                this.mapComponent =
                    mapComponent ?? throw new ArgumentNullException(nameof(mapComponent));
            }

            public void Add(IGuiComponent component)
            {
                this.inner.Add(this.mapComponent(component));
            }
        }
    }
}

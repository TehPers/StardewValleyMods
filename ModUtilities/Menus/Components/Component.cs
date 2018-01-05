using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Enums;
using ModUtilities.Helpers;
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;
using Rectangle2 = Microsoft.Xna.Framework.Rectangle;

namespace ModUtilities.Menus.Components {
    public abstract class Component {
        private const float DrawDepthStart = 0.5f;
        private const float DrawDepthRange = 0.01f;

        /// <summary>The focused component. This is ignored for everything except the topmost parent component.</summary>
        private Component _focused;

        /// <summary>Parent of this <see cref="Component"/></summary>
        public Component Parent { get; set; }
        /// <summary>Children of this <see cref="Component"/></summary>
        public ISet<Component> Children { get; } = new HashSet<Component>();
        /// <summary>Bounds relative to <see cref="Parent"/>, or the viewport if <see cref="Parent"/> is null</summary>
        public Rectangle Bounds {
            get => new Rectangle(this.Location, this.Size);
            set {
                this.Location = value.Location;
                this.Size = value.Size;
            }
        }
        /// <summary>Location relative to <see cref="Parent"/>, or the viewport if <see cref="Parent"/> is null</summary>
        public Location Location { get; set; } = new Location(0, 0);
        /// <summary>The origin point for child controls</summary>
        public Location ChildOrigin => this.ChildBounds.Location;
        /// <summary>The bounds that child controls should fit inside of</summary>
        public virtual Rectangle ChildBounds => new Rectangle(Location.Origin, this.Size);
        /// <summary>Size of this <see cref="Component"/></summary>
        public Size Size { get; set; } = new Size(20, 20);
        /// <summary>Whether this component should be drawn. It will still have the same bounds and receive events</summary>
        public bool Visible { get; set; } = true;
        /// <summary>Whether this component should be enabled and receive events</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>The text to display when this component is hovered over by the mouse</summary>
        public string HoverText { get; } = null;
        /// <summary>What depth to draw this component at</summary>
        private float DrawDepth => this.Parent?.DrawDepth - Component.DrawDepthRange ?? Component.DrawDepthStart;
        /// <summary>Location relative to the viewport</summary>
        public Location AbsoluteLocation {
            get => this.Parent == null ? this.Location : this.Parent.AbsoluteLocation + this.Parent.ChildOrigin + this.Location;
            set => this.Location = this.Parent == null ? value : value - this.Parent.AbsoluteLocation - this.Parent.ChildOrigin;
        }
        /// <summary>Bounds relative to the viewport</summary>
        public Rectangle AbsoluteBounds {
            get => new Rectangle(this.AbsoluteLocation, this.Size);
            set {
                this.AbsoluteLocation = value.Location;
                this.Size = value.Size;
            }
        }
        /// <summary>Whether this control should be focused when it's clicked</summary>
        public virtual bool FocusOnClick { get; } = false;
        /// <summary>Bounds this control can be clicked in to get focused</summary>
        public virtual Rectangle FocusBounds => this.AbsoluteBounds;
        /// <summary>Whether this control is focused</summary>
        public bool IsFocused => this.GetFocusedComponent() == this;

        protected Component() { }

        protected Component(Component parent) : this() {
            this.Parent = parent;
        }

        public Component AddChild(Component child) {
            // Update previous parent if it had one
            child.Parent?.Children.Remove(child);

            // Update child
            child.Parent = this;

            // Update this
            this.Children.Add(child);

            return this;
        }

        #region Events
        /// <summary>Draws the component</summary>
        /// <param name="b">The <see cref="SpriteBatch"/> used for drawing to the screen</param>
        public virtual void Draw(SpriteBatch b) {
            if (!this.Visible)
                return;

            // Draw control
            this.OnDraw(b);

            // Draw children
            foreach (Component child in this.Children) {
                child.Draw(b);
            }
        }

        /// <summary>Click the control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="btn">The mouse button that was clicked</param>
        /// <returns>Whether the mouse click was handled by this component</returns>
        public virtual bool Click(Location mousePos, MouseButtons btn) {
            // Check if component is enabled
            if (!this.Enabled)
                return false;

            // Try to have children handle the click
            if (this.Children.Any(child => child.Click(mousePos, btn)))
                return true;

            // If the click was outside this component's bounds, don't handle it
            if (!this.AbsoluteBounds.Contains(mousePos))
                return false;

            // Handle the click
            switch (btn) {
                case MouseButtons.LEFT:
                    return this.OnLeftClick(mousePos);
                case MouseButtons.RIGHT:
                    return this.OnRightClick(mousePos);
            }

            // Somehow not a right or left click
            return false;
        }

        /// <summary>Scroll the mouse over the component</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="direction">The direction being scrolled in</param>
        /// <returns>Whether the scroll was handled by this component</returns>
        public virtual bool Scroll(Location mousePos, int direction) {
            // Check if component is enabled
            if (!this.Enabled)
                return false;

            // Pass scroll to children
            if (this.Children.Any(child => child.Scroll(mousePos, direction)))
                return true;

            // Try to handle the scroll
            return this.AbsoluteBounds.Contains(mousePos) && this.OnScroll(mousePos, direction);
        }

        /// <summary>Drag left click over the component</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <returns>Whether the event was handled by this component</returns>
        public virtual bool Drag(Location mousePos) {
            // Check if component is enabled
            if (!this.Enabled)
                return false;

            // Try to have children handle the click
            if (this.Children.Any(child => child.Drag(mousePos)))
                return true;

            // If the click was outside this component's bounds, don't handle it
            return this.AbsoluteBounds.Contains(mousePos) && this.OnDrag(mousePos);
        }

        /// <summary>Send a key pressed event to the component and all child components</summary>
        /// <param name="key">The key being pressed</param>
        /// <returns>Whether propagation should stop</returns>
        public virtual bool PressKey(Keys key) => this.Enabled && this.OnKeyPressed(key) || (this.Parent?.PressKey(key) ?? false);

        /// <summary>Send some text to the component and all child components</summary>
        /// <param name="text">The text to send to the component</param>
        /// <returns>Whether propagation should stop</returns>
        public virtual bool EnterText(string text) => this.Enabled && this.OnTextEntered(text) || (this.Parent?.EnterText(text) ?? false);
        #endregion

        #region Event handlers
        /// <summary>Called when this component should draw itself</summary>
        /// <param name="b">The <see cref="SpriteBatch"/> used for drawing to the screen</param>
        protected abstract void OnDraw(SpriteBatch b);

        /// <summary>Called when this component is left clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnLeftClick(Location mousePos) {
            // Focus this control if supposed to
            if (this.FocusOnClick && this.FocusBounds.Contains(mousePos)) {
                this.Focus();
                return true;
            }

            return false;
        }

        /// <summary>Called when this component is right clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnRightClick(Location mousePos) {
            // Focus this control if supposed to
            if (this.FocusOnClick && this.FocusBounds.Contains(mousePos)) {
                this.Focus();
                return true;
            }

            return false;
        }

        /// <summary>Called whenever the mouse is scrolled over this component</summary>
        /// <param name="mousePos"></param>
        /// <param name="direction"></param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnScroll(Location mousePos, int direction) => false;

        /// <summary>Called whenever a key is pressed</summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnKeyPressed(Keys key) => false;

        /// <summary>Called whenever the left mouse button is dragged over this control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnDrag(Location mousePos) => false;

        /// <summary>Called whenever text is sent to this control</summary>
        /// <param name="text">The text that was entered</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnTextEntered(string text) => false;
        #endregion

        /// <summary>Gets the currently focused component</summary>
        /// <returns>The currently focused component</returns>
        public Component GetFocusedComponent() {
            if (this.Parent != null)
                return this.Parent.GetFocusedComponent();

            return this._focused?.Enabled == true ? this._focused : null;
        }

        /// <summary>Focuses this component</summary>
        public void Focus() => this.Focus(this);

        /// <summary>Focuses the given component</summary>
        /// <param name="component">The component to focus</param>
        public void Focus(Component component) {
            if (this.Parent == null) {
                this._focused = component;
            } else {
                this.Parent.Focus(component);
            }
        }

        /// <summary>Converts a local depth [0, 1] to a global depth</summary>
        /// <param name="localDepth">Value between 0 and 1 (inclusive) for use when planning what gets drawn on top of what. Higher values are drawn over lower values</param>
        /// <returns>The global depth, for use in draw calls and such</returns>
        public float GetGlobalDepth(float localDepth) => this.DrawDepth - Component.DrawDepthRange * localDepth.Clamp(0, 1);

        protected void WithScissorRect(SpriteBatch batch, Rectangle2 scissorRect, Action<SpriteBatch> action, bool respectExistingScissor = true) {
            // Keep track of the old scissor rectangle
            Rectangle2 oldScissor = batch.GraphicsDevice.ScissorRectangle;
            BlendState oldBlendState = batch.GraphicsDevice.BlendState;
            DepthStencilState oldStencilState = batch.GraphicsDevice.DepthStencilState;
            RasterizerState oldRasterizerState = batch.GraphicsDevice.RasterizerState;

            // Trim current scissor to the existing one if necessary
            if (respectExistingScissor)
                scissorRect = scissorRect.Intersection(oldScissor) ?? new Rectangle2(0, 0, 0, 0);

            // Setup temporary sprite batch to use the scissor rectangle
            batch.End();
            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, new RasterizerState { ScissorTestEnable = true });

            // Set scissor rectangle
            batch.GraphicsDevice.ScissorRectangle = scissorRect;

            // Perform the action
            action(batch);

            // Draw the batch
            batch.End();
            batch.Begin(SpriteSortMode.BackToFront, oldBlendState, SamplerState.PointClamp, oldStencilState, oldRasterizerState);

            // Reset scissor rectangle
            batch.GraphicsDevice.ScissorRectangle = oldScissor;
        }
    }
}
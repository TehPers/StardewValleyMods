using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Enums;
using ModUtilities.Helpers;
using StardewValley;

namespace ModUtilities.Menus.Components2 {
    public abstract class Component2 {
        private const float DrawDepthStart = 0.5f;
        private const float DrawDepthRange = 0.01f;

        /// <summary>The focused component. This is ignored for everything except the topmost parent component.</summary>
        private Component2 _focused;

        /// <summary>Parent of this <see cref="Component2"/></summary>
        public Component2 Parent { get; set; }
        /// <summary>Children of this <see cref="Component2"/></summary>
        public ISet<Component2> Children { get; } = new HashSet<Component2>();

        /// <summary>Location relative to <see cref="Parent"/>, or the viewport if <see cref="Parent"/> is null</summary>
        public virtual RelativeLocation Location { get; set; } = RelativeLocation.TopLeft;
        /// <summary>Size of this <see cref="Component2"/></summary>
        public virtual RelativeSize Size { get; set; } = new RelativeSize(0, 0, 20, 20);
        /// <summary>Bounds relative to <see cref="Parent"/>, or the viewport if <see cref="Parent"/> is null</summary>
        public RelativeRectangle Bounds {
            get => new RelativeRectangle(this.Location, this.Size);
            set {
                this.Location = value.Location;
                this.Size = value.Size;
            }
        }
        /// <summary>Bounds in absolute coordinates</summary>
        public Rectangle AbsoluteBounds => this.GetAbsoluteRectangle(this.Bounds);
        /// <summary>Location in absolute coordinates</summary>
        public Point AbsoluteLocation => this.AbsoluteBounds.Location;

        /// <summary>The origin point for children</summary>
        public RelativeLocation ChildOrigin => this.ChildBounds.Location;
        /// <summary>The bounds that children should fit inside of</summary>
        public virtual RelativeRectangle ChildBounds => new RelativeRectangle(RelativeLocation.TopLeft, RelativeSize.Fill);
        /// <summary>The absolute bounds that children should fit inside of</summary>
        public Rectangle AbsoluteChildBounds => this.ChildBounds.ToAbsolute(this.AbsoluteBounds);

        /// <summary>Whether this component should be drawn. If this is false, the component will not receive events.</summary>
        public bool Visible { get; set; } = true;
        /// <summary>Whether this component should be enabled and receive events</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>The text to display when this component is hovered over by the mouse</summary>
        public string HoverText { get; } = null;
        /// <summary>What depth to draw this component at</summary>
        private float DrawDepth => this.Parent?.DrawDepth - Component2.DrawDepthRange ?? Component2.DrawDepthStart;

        /// <summary>Whether this control should be focused when it's clicked</summary>
        public virtual bool FocusOnClick { get; } = false;
        /// <summary>Bounds this control can be clicked in to get focused</summary>
        public virtual RelativeRectangle FocusBounds => this.Bounds;
        /// <summary>Absolute bounds this control can be clicked in to get focused</summary>
        public Rectangle AbsoluteFocusBounds => this.GetAbsoluteRectangle(this.FocusBounds);
        /// <summary>Whether this control is focused</summary>
        public bool IsFocused => this.GetFocusedComponent() == this;
        /// <summary>Whether this or any of its descendents are focused</summary>
        public bool IsThisOrChildrenFocused => this.IsFocused || this.Children.Any(c => c.IsFocused);

        protected Component2() { }

        protected Component2(Component2 parent) : this() {
            this.Parent = parent;
        }

        public Component2 AddChild(Component2 child) {
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
            foreach (Component2 child in this.Children) {
                child.Draw(b);
            }
        }

        /// <summary>Click the control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="btn">The mouse button that was clicked</param>
        /// <returns>Whether the mouse click was handled by this component</returns>
        public virtual bool Click(Point mousePos, MouseButtons btn) {
            // Check if component is enabled
            if (!this.Enabled || !this.Visible)
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
        public virtual bool Scroll(Point mousePos, int direction) {
            // Check if component is enabled
            if (!this.Enabled || !this.Visible)
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
        public virtual bool Drag(Point mousePos) {
            // Check if component is enabled
            if (!this.Enabled || !this.Visible)
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
        public virtual bool PressKey(Keys key) => this.Enabled && this.Visible && this.OnKeyPressed(key) || (this.Parent?.PressKey(key) ?? false);

        /// <summary>Send some text to the component and all child components</summary>
        /// <param name="text">The text to send to the component</param>
        /// <returns>Whether propagation should stop</returns>
        public virtual bool EnterText(string text) => this.Enabled && this.Visible && this.OnTextEntered(text) || (this.Parent?.EnterText(text) ?? false);
        #endregion

        #region Event handlers
        /// <summary>Called when this component should draw itself</summary>
        /// <param name="b">The <see cref="SpriteBatch"/> used for drawing to the screen</param>
        protected abstract void OnDraw(SpriteBatch b);

        /// <summary>Called when this component is left clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnLeftClick(Point mousePos) {
            // Focus this control if supposed to
            if (this.FocusOnClick && this.AbsoluteFocusBounds.Contains(mousePos)) {
                this.Focus();
                return true;
            }

            return false;
        }

        /// <summary>Called when this component is right clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnRightClick(Point mousePos) {
            // Focus this control if supposed to
            if (this.FocusOnClick && this.AbsoluteFocusBounds.Contains(mousePos)) {
                this.Focus();
                return true;
            }

            return false;
        }

        /// <summary>Called whenever the mouse is scrolled over this component</summary>
        /// <param name="mousePos"></param>
        /// <param name="direction"></param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnScroll(Point mousePos, int direction) => false;

        /// <summary>Called whenever a key is pressed</summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnKeyPressed(Keys key) => false;

        /// <summary>Called whenever the left mouse button is dragged over this control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnDrag(Point mousePos) => false;

        /// <summary>Called whenever text is sent to this control</summary>
        /// <param name="text">The text that was entered</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnTextEntered(string text) => false;
        #endregion

        /// <summary>Gets the currently focused component</summary>
        /// <returns>The currently focused component</returns>
        public Component2 GetFocusedComponent() {
            if (this.Parent != null)
                return this.Parent.GetFocusedComponent();

            return this._focused?.Enabled == true ? this._focused : null;
        }

        /// <summary>Focuses this component</summary>
        public void Focus() => this.Focus(this);

        /// <summary>Focuses the given component</summary>
        /// <param name="component">The component to focus</param>
        public void Focus(Component2 component) {
            if (this.Parent == null) {
                this._focused = component;
            } else {
                this.Parent.Focus(component);
            }
        }

        /// <summary>Converts a local depth [0, 1] to a global depth</summary>
        /// <param name="localDepth">Value between 0 and 1 (inclusive) for use when planning what gets drawn on top of what. Higher values are drawn over lower values</param>
        /// <returns>The global depth, for use in draw calls and such</returns>
        public float GetGlobalDepth(float localDepth) => this.DrawDepth - Component2.DrawDepthRange * localDepth.Clamp(0, 1);

        /// <summary>Creates a <see cref="Rectangle"/> based on a <see cref="RelativeRectangle"/> relative to this</summary>
        /// <param name="relative">The <see cref="RelativeRectangle"/> to convert</param>
        /// <returns>The <see cref="Rectangle"/> created from the given <see cref="RelativeRectangle"/></returns>
        public Rectangle GetAbsoluteRectangle(RelativeRectangle relative) {
            return relative.ToAbsolute(this.Parent?.AbsoluteChildBounds ?? new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height));
        }

        /// <summary>Executes some code with a given scissor rectangle (drawing will be limited to the rectangle)</summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> being used for drawing calls</param>
        /// <param name="scissorRect">The rectangle to limit drawing to</param>
        /// <param name="action">The function to execute with the given scissor rectangle</param>
        /// <param name="respectExistingScissor">Whether to limit the new scissor rectangle to a subrectangle of the current scissor rectangle</param>
        protected void WithScissorRect(SpriteBatch batch, Rectangle scissorRect, Action<SpriteBatch> action, bool respectExistingScissor = true) {
            // Stop the old drawing code
            batch.End();

            // Keep track of the old scissor rectangle (this needs to come after End() so they're applied to the GraphicsDevice)
            Rectangle oldScissor = batch.GraphicsDevice.ScissorRectangle;
            BlendState oldBlendState = batch.GraphicsDevice.BlendState;
            DepthStencilState oldStencilState = batch.GraphicsDevice.DepthStencilState;
            RasterizerState oldRasterizerState = batch.GraphicsDevice.RasterizerState;

            // Trim current scissor to the existing one if necessary
            if (respectExistingScissor)
                scissorRect = scissorRect.Intersection(oldScissor) ?? new Rectangle(0, 0, 0, 0);

            // Draw with the new scissor rectangle
            using (RasterizerState rasterizerState = new RasterizerState { ScissorTestEnable = true }) {
                batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);

                // Set scissor rectangle
                batch.GraphicsDevice.ScissorRectangle = scissorRect;

                // Perform the action
                action(batch);

                // Draw the batch
                batch.End();
            }

            // Reset scissor rectangle
            batch.GraphicsDevice.ScissorRectangle = oldScissor;

            // Return to last state
            batch.Begin(SpriteSortMode.BackToFront, oldBlendState, SamplerState.PointClamp, oldStencilState, oldRasterizerState);
        }
    }
}
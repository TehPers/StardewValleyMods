using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TehCore.Enums;
using TehCore.Helpers;
using TehCore.Menus.BoxModel;

namespace TehCore.Menus.Elements {
    public abstract class Element {
        private const float DrawDepthStart = 0.5f;
        private const float DrawDepthRange = 0.01f;

        /// <summary>The focused component. This is ignored for everything except the topmost parent component.</summary>
        private Element _focused;

        /* Size properties */
        public OuterSize Margin { get; set; } = OuterSize.Zero;
        public OuterSize Padding { get; set; } = OuterSize.Zero;

        public Element Parent { get; set; } = null;
        public List<Element> Children { get; } = new List<Element>();

        /// <summary>Location relative to <see cref="Parent"/>, or the viewport if <see cref="Parent"/> is null</summary>
        public virtual BoxVector Location { get; set; } = BoxVector.Zero;
        /// <summary>Size of this <see cref="Element"/></summary>
        public virtual BoxVector Size { get; set; } = BoxVector.Fill;
        /// <summary>Bounds relative to <see cref="Parent"/>, or the viewport if <see cref="Parent"/> is null</summary>
        public BoxRectangle Bounds {
            get => new BoxRectangle(this.Location, this.Size);
            set {
                this.Location = value.Location;
                this.Size = value.Size;
            }
        }

        /// <summary>Whether this control should be focused when it's clicked</summary>
        public virtual bool FocusOnClick { get; } = false;
        /// <summary>Bounds this control can be clicked in to get focused</summary>
        public virtual BoxRectangle FocusBounds => this.Bounds;
        /// <summary>Whether this control is focused</summary>
        public bool IsFocused => this.GetFocusedComponent() == this;
        /// <summary>Whether this or any of its descendents are focused</summary>
        public bool IsThisOrChildrenFocused => this.IsFocused || this.Children.Any(c => c.IsFocused);

        /// <summary>Whether this component should be drawn. If this is false, the component will not receive events.</summary>
        public bool Visible { get; set; } = true;
        /// <summary>Whether this component should be enabled and receive events</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>The text to display when this component is hovered over by the mouse</summary>
        public string HoverText { get; } = null;
        /// <summary>What depth to draw this component at</summary>
        private float DrawDepth => this.Parent?.DrawDepth - Element.DrawDepthRange ?? Element.DrawDepthStart;

        protected Element() { }

        protected Element(Element parent) : this() {
            this.Parent = parent;
        }

        public Element AddChild(Element child) {
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
        /// <param name="batch">The <see cref="SpriteBatch"/> used for drawing to the screen</param>
        /// <param name="parentSize">The size of the parent element</param>
        public void Draw(SpriteBatch batch, Vector2I parentSize) {
            if (!this.Visible)
                return;

            // Draw control
            this.OnDraw(batch, parentSize);

            // Draw children
            foreach (Element child in this.Children) {
                child.Draw(batch, this.Size.ToAbsolute(parentSize));
            }
        }

        /// <summary>Click the control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="buttons">The mouse button that was clicked</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <returns>Whether the mouse click was handled by this component</returns>
        public bool Click(Point mousePos, MouseButtons buttons, Vector2I parentSize) {
            // Check if component is enabled
            if (!this.Enabled || !this.Visible)
                return false;

            // Try to have children handle the click
            if (this.Children.Any(child => child.Click(mousePos, buttons, parentSize)))
                return true;

            // If the click was outside this component's bounds, don't handle it
            if (!this.Bounds.ToAbsolute(parentSize).Contains(mousePos))
                return false;

            // Handle the click
            switch (buttons) {
                case MouseButtons.LEFT:
                    return this.OnLeftClick(mousePos, parentSize);
                case MouseButtons.RIGHT:
                    return this.OnRightClick(mousePos, parentSize);
            }

            // Somehow not a right or left click
            return false;
        }

        // <summary>Scroll the mouse over the component</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="direction">The direction being scrolled in</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <returns>Whether the scroll was handled by this component</returns>
        public bool Scroll(Point mousePos, int direction, Vector2I parentSize) {
            // Check if component is enabled
            if (!this.Enabled || !this.Visible)
                return false;

            // Pass scroll to children
            if (this.Children.Any(child => child.Scroll(mousePos, direction, parentSize)))
                return true;

            // Try to handle the scroll
            return this.Bounds.ToAbsolute(parentSize).Contains(mousePos) && this.OnScroll(mousePos, direction, parentSize);
        }

        /// <summary>Drag left click over the component</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <returns>Whether the event was handled by this component</returns>
        public bool Drag(Point mousePos, Vector2I parentSize) {
            // Check if component is enabled
            if (!this.Enabled || !this.Visible)
                return false;

            // Try to have children handle the click
            if (this.Children.Any(child => child.Drag(mousePos, parentSize)))
                return true;

            // If the click was outside this component's bounds, don't handle it
            return this.Bounds.ToAbsolute(parentSize).Contains(mousePos) && this.OnDrag(mousePos, parentSize);
        }

        /// <summary>Send a key pressed event to the component and all child components</summary>
        /// <param name="key">The key being pressed</param>
        /// <returns>Whether propagation should stop</returns>
        public bool PressKey(Keys key) => this.Enabled && this.Visible && this.OnKeyPressed(key) || (this.Parent?.PressKey(key) ?? false);

        /// <summary>Send some text to the component and all child components</summary>
        /// <param name="text">The text to send to the component</param>
        /// <returns>Whether propagation should stop</returns>
        public bool EnterText(string text) => this.Enabled && this.Visible && this.OnTextEntered(text) || (this.Parent?.EnterText(text) ?? false);
        #endregion

        #region Event Handlers
        /// <summary>Called when this component should draw itself</summary>
        /// <param name="b">The <see cref="SpriteBatch"/> used for drawing to the screen</param>
        /// <param name="parentSize">The size of the parent element</param>
        protected abstract void OnDraw(SpriteBatch b, Vector2I parentSize);

        /// <summary>Called when this component is left clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnLeftClick(Point mousePos, Vector2I parentSize) {
            // Focus this control if supposed to
            if (this.FocusOnClick && this.FocusBounds.ToAbsolute(parentSize).Contains(mousePos)) {
                this.Focus();
                return true;
            }

            return false;
        }

        /// <summary>Called when this component is right clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnRightClick(Point mousePos, Vector2I parentSize) {
            // Focus this control if supposed to
            if (this.FocusOnClick && this.FocusBounds.ToAbsolute(parentSize).Contains(mousePos)) {
                this.Focus();
                return true;
            }

            return false;
        }

        /// <summary>Called whenever the mouse is scrolled over this component</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <param name="direction">The direction and amount of scroll</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnScroll(Point mousePos, int direction, Vector2I parentSize) => false;

        /// <summary>Called whenever a key is pressed</summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnKeyPressed(Keys key) => false;

        /// <summary>Called whenever the left mouse button is dragged over this control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="parentSize">The size of the parent element</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnDrag(Point mousePos, Vector2I parentSize) => false;

        /// <summary>Called whenever text is sent to this control</summary>
        /// <param name="text">The text that was entered</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnTextEntered(string text) => false;
        #endregion

        #region Focus
        /// <summary>Gets the currently focused component</summary>
        /// <returns>The currently focused component</returns>
        public Element GetFocusedComponent() {
            if (this.Parent != null)
                return this.Parent.GetFocusedComponent();

            return this._focused?.Enabled == true ? this._focused : null;
        }

        /// <summary>Focuses this component</summary>
        public void Focus() => this.Focus(this);

        /// <summary>Focuses the given component</summary>
        /// <param name="component">The component to focus</param>
        public void Focus(Element component) {
            if (this.Parent == null) {
                this._focused = component;
            } else {
                this.Parent.Focus(component);
            }
        }
        #endregion

        #region Draw Helpers
        /// <summary>Converts a local depth [0, 1] to a global depth</summary>
        /// <param name="localDepth">Value between 0 and 1 (inclusive) for use when planning what gets drawn on top of what. Higher values are drawn over lower values</param>
        /// <returns>The global depth, for use in draw calls and such</returns>
        public float GetGlobalDepth(float localDepth) => this.DrawDepth - Element.DrawDepthRange * localDepth.Clamp(0, 1);
        
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
        #endregion
    }
}

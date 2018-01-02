using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Enums;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ModUtilities.Menus.Components {
    public abstract class Component {
        /// <summary>The focused component. This is ignored for everything except the topmost parent component.</summary>
        private Component _focused;

        /// <summary>Parent of this <see cref="Component"/></summary>
        public Component Parent { get; set; }
        /// <summary>Children of this <see cref="Component"/></summary>
        public IList<Component> Children { get; } = new List<Component>();
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
        public virtual Location ChildOrigin { get; } = new Location(0, 0);
        /// <summary>Size of this <see cref="Component"/></summary>
        public Size Size { get; set; } = new Size(20, 20);
        /// <summary>Whether this component should be drawn. It will still have the same bounds and receive events</summary>
        public bool Visible { get; set; } = true;
        /// <summary>Whether this component should be enabled and receive events</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>The text to display when this component is hovered over by the mouse</summary>
        public string HoverText { get; } = null;

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
        /// <summary>Whether this control is focused</summary>
        public bool IsFocused => this.GetFocusedComponent() == this;

        protected Component() {

        }

        protected Component(Component parent) {
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
        /// <param name="playSound">Whether to play sounds</param>
        /// <param name="btn">The mouse button that was clicked</param>
        /// <returns>Whether the mouse click was handled by this component</returns>
        public bool Click(Location mousePos, bool playSound, MouseButtons btn) {
            // Check if click was out of bounds
            if (!this.Enabled || !this.AbsoluteBounds.Contains(mousePos))
                return false;

            // Focus this control if supposed to
            if (this.FocusOnClick)
                this.Focus();

            // Pass click to children
            bool childHandled = this.Children.Any(child => child.Click(mousePos, playSound, btn));

            // Handle the click if it wasn't handled by a child component
            if (!childHandled) {
                switch (btn) {
                    case MouseButtons.LEFT:
                        this.OnLeftClick(mousePos, playSound);
                        break;
                    case MouseButtons.RIGHT:
                        this.OnRightClick(mousePos, playSound);
                        break;
                }
            }

            // Click was handled
            return true;
        }

        /// <summary>Scroll the mouse over the control</summary>
        /// <param name="mousePos">The position of the mouse</param>
        /// <param name="direction">The direction being scrolled in</param>
        /// <returns>Whether the scroll was handled by this component</returns>
        public bool Scroll(Location mousePos, int direction) {
            // Check if mouse was out of bounds
            if (!this.Enabled || this.AbsoluteBounds.Contains(mousePos))
                return false;

            // Pass scroll to children
            bool childHandled = this.Children.Any(child => child.Scroll(mousePos, direction));

            // Handle the click if it wasn't handled by a child component
            if (!childHandled) {
                this.OnScroll(mousePos, direction);
            }

            // Click was handled
            return true;
        }

        /// <summary>Send a key pressed event to the component and all child components</summary>
        /// <param name="key">The key being pressed</param>
        /// <returns>Whether propagation should stop</returns>
        public bool PressKey(Keys key) {
            return (this.Enabled && this.OnKeyPressed(key)) || (this.Parent?.PressKey(key) ?? false);
        }

        /// <summary>Send some text to the component and all child components</summary>
        /// <param name="text">The text to send to the component</param>
        /// <returns>Whether propagation should stop</returns>
        public bool EnterText(string text) {
            return (this.Enabled && this.OnTextEntered(text)) || (this.Parent?.EnterText(text) ?? false);
        }
        #endregion

        #region Event handlers
        /// <summary>Called when this component should draw itself</summary>
        /// <param name="b">The <see cref="SpriteBatch"/> used for drawing to the screen</param>
        protected abstract void OnDraw(SpriteBatch b);

        /// <summary>Called when this component is left clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <param name="playSound">Whether to play sound</param>
        protected virtual void OnLeftClick(Location mousePos, bool playSound) { }

        /// <summary>Called when this component is right clicked</summary>
        /// <param name="mousePos">The location of the mouse</param>
        /// <param name="playSound">Whether to play sound</param>
        protected virtual void OnRightClick(Location mousePos, bool playSound) { }

        /// <summary>Called whenever the mouse is scrolled over this component</summary>
        /// <param name="mousePos"></param>
        /// <param name="direction"></param>
        protected virtual void OnScroll(Location mousePos, int direction) { }

        /// <summary>Called whenever a key is pressed</summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>Whether to stop propagation of the event</returns>
        protected virtual bool OnKeyPressed(Keys key) => false;

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
    }
}
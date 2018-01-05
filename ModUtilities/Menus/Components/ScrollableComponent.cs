using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ModUtilities.Enums;
using ModUtilities.Helpers;
using StardewValley;
using xTile.Dimensions;

namespace ModUtilities.Menus.Components {
    public class ScrollableComponent : Component {
        private readonly ScrollbarComponent _scrollbar;

        public bool AutoScrollbar { get; set; } = true;
        public bool ScrollbarVisible { get => this._scrollbar.Visible; set => this._scrollbar.Visible = this._scrollbar.Enabled = value; }
        public int PixelsPerScroll { get => this._scrollbar.ScrollSpeed; set => this._scrollbar.ScrollSpeed = value; }
        public int ScrollbarPadding { get; set; } = 12;

        public override Rectangle ChildBounds => new Rectangle(0, -(this._scrollbar?.Value ?? 0), this.Size.Width - (this._scrollbar?.Size.Width + this.ScrollbarPadding ?? 0), this.Size.Height);

        public ScrollableComponent() {
            this._scrollbar = new ScrollbarComponent(false)
                .Chain(c => c.ScrollSpeed = ModUtilities.Instance.Config.ScrollSpeed);
            this.AddChild(this._scrollbar);
        }

        public override void Draw(SpriteBatch batch) {
            if (!this.Visible)
                return;

            Rectangle bounds = this.AbsoluteBounds;

            // Draw the component without the scrollbar
            this.Children.Remove(this._scrollbar);
            this.WithScissorRect(batch, new Microsoft.Xna.Framework.Rectangle(bounds.X, bounds.Y, this.ChildBounds.Width, bounds.Height), b => base.Draw(b));
            this.Children.Add(this._scrollbar);

            // Draw the scrollbar
            this._scrollbar.Draw(batch);
        }

        protected override void OnDraw(SpriteBatch b) {
            // Draw background
            this.OnDrawBackground(b);

            // Get total height of all the components
            int totalHeight = this.Children.Max(c => c.Location.Y + c.Size.Height);

            // Update scrollbar
            Rectangle childBounds = this.ChildBounds;
            this._scrollbar.Maximum = Math.Max(totalHeight - this.Size.Height, 0);
            this._scrollbar.Location = new Location(this.Size.Width - this._scrollbar.Size.Width, -childBounds.Y);
            this._scrollbar.Size = new Size(this._scrollbar.Size.Width, childBounds.Height);
        }

        public override bool Click(Location mousePos, MouseButtons btn) => this.AbsoluteBounds.Contains(mousePos) && base.Click(mousePos, btn);

        public override bool Drag(Location mousePos) => this.AbsoluteBounds.Contains(mousePos) && base.Drag(mousePos);

        public override bool Scroll(Location mousePos, int direction) => this.AbsoluteBounds.Contains(mousePos) && base.Scroll(mousePos, direction);

        protected virtual void OnDrawBackground(SpriteBatch obj) => this.DrawBackground?.Invoke(obj);

        protected override bool OnScroll(Location mousePos, int direction) => this._scrollbar.Scroll(this._scrollbar.AbsoluteLocation, direction);

        public event Action<SpriteBatch> DrawBackground;
    }
}

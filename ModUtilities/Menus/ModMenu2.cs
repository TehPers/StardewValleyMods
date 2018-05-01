using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Enums;
using ModUtilities.Helpers;
using ModUtilities.Menus.Components;
using ModUtilities.Menus.Components2;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModMenu2 : IClickableMenu {
        public MenuComponent2 Component { get; } = new MenuComponent2();

        public ModMenu2(RelativeRectangle bounds) : base(0, 0, 200, 200) {
            this.Component.Bounds = bounds;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (!this.Component.Click(new Point(x, y), MouseButtons.LEFT))
                base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            this.Component.Click(new Point(x, y), MouseButtons.RIGHT);
        }

        public override void receiveScrollWheelAction(int direction) {
            this.Component.Scroll(new Point(Game1.getMouseX(), Game1.getMouseY()), direction);
        }

        public override void receiveKeyPress(Keys key) {
            Component2 focused = this.Component.GetFocusedComponent() ?? this.Component;
            if (!focused.PressKey(key))
                base.receiveKeyPress(key);
        }

        public override void leftClickHeld(int x, int y) {
            if (!this.Component.Drag(new Point(x, y)))
                base.leftClickHeld(x, y);
        }

        public void EnterText(string text) {
            Component2 focused = this.Component.GetFocusedComponent() ?? this.Component;
            focused.EnterText(text);
        }

        public override void performHoverAction(int x, int y) {
            //this._hoverText = "";
            //this._hoverTitle = "";
        }

        protected virtual void DrawCursor(SpriteBatch b) {}

        protected virtual void DrawHoverText(SpriteBatch b) {
            //IClickableMenu.drawHoverText(b, this._hoverText, Game1.smallFont, 0, 0, -1, this._hoverTitle.Length > 0 ? this._hoverTitle : null);
        }

        public override void draw(SpriteBatch b) {
            if (this.Component == null)
                return;

            Microsoft.Xna.Framework.Rectangle absoluteBounds = this.Component.AbsoluteBounds;
            this.xPositionOnScreen = absoluteBounds.X;
            this.yPositionOnScreen = absoluteBounds.Y;
            this.width = absoluteBounds.Width;
            this.height = absoluteBounds.Height;

            using (SpriteBatch menuBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                menuBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                this.Component.Draw(b);
                this.Component.DrawCursor(b);
                menuBatch.End();
            }
        }
    }
}

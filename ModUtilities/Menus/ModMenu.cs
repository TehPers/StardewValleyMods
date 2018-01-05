using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Enums;
using ModUtilities.Menus.Components;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModMenu : IClickableMenu {
        public MenuComponent Component { get; } = new MenuComponent();

        public ModMenu(int x, int y, int width, int height) : base(x, y, width, height) {
            this.Component.Bounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (!this.Component.Click(new Location(x, y), MouseButtons.LEFT))
                base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            this.Component.Click(new Location(x, y), MouseButtons.RIGHT);
        }

        public override void receiveScrollWheelAction(int direction) {
            this.Component.Scroll(new Location(Game1.getMouseX(), Game1.getMouseY()), direction);
        }

        public override void receiveKeyPress(Keys key) {
            Component focused = this.Component.GetFocusedComponent() ?? this.Component;
            if (!focused.PressKey(key))
                base.receiveKeyPress(key);
        }

        public override void leftClickHeld(int x, int y) {
            if (!this.Component.Drag(new Location(x, y)))
                base.leftClickHeld(x, y);
        }

        public void EnterText(string text) {
            Component focused = this.Component.GetFocusedComponent() ?? this.Component;
            focused.EnterText(text);
        }

        public override void performHoverAction(int x, int y) {
            //this._hoverText = "";
            //this._hoverTitle = "";
        }

        protected virtual void DrawCursor(SpriteBatch b) {
            // Do nothing, the MenuComponent handles it
        }

        protected virtual void DrawHoverText(SpriteBatch b) {
            //IClickableMenu.drawHoverText(b, this._hoverText, Game1.smallFont, 0, 0, -1, this._hoverTitle.Length > 0 ? this._hoverTitle : null);
        }

        public override void draw(SpriteBatch b) {
            if (this.Component == null)
                return;

            using (SpriteBatch menuBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                menuBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                this.Component.Draw(b);
                this.Component.DrawCursor(b);
            }
        }
    }
}

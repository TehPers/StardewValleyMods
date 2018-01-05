using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Helpers;
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;
using Rectangle2 = Microsoft.Xna.Framework.Rectangle;

namespace ModUtilities.Menus.Components {
    public class MenuComponent : Component {
        public override Rectangle ChildBounds => new Rectangle(base.ChildBounds.X + Game1.tileSize, base.ChildBounds.Y + Game1.tileSize, this.Size.Width - 2 * Game1.tileSize, this.Size.Height - 2 * Game1.tileSize);

        public virtual bool StopKeyPropagation { get; set; } = false;

        public MenuComponent() { }

        protected virtual void DrawBackground(SpriteBatch b) {
            b.DrawMenuBox(this.AbsoluteBounds.ToXnaRectangle(), this.GetGlobalDepth(0));
        }

        public virtual void DrawCursor(SpriteBatch b) {
            if (Game1.options.hardwareCursor)
                return;
            b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
        }

        protected override void OnDraw(SpriteBatch b) {
            this.DrawBackground(b);
        }

        protected override bool OnKeyPressed(Keys key) => this.StopKeyPropagation;
    }
}

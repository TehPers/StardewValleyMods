using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Helpers;
using StardewValley;

namespace ModUtilities.Menus.Components2 {
    public class MenuComponent2 : Component2 {
        public override RelativeRectangle ChildBounds => RelativeRectangle.FromOffset(Game1.tileSize, Game1.tileSize, -2 * Game1.tileSize, -2 * Game1.tileSize);

        public virtual bool StopKeyPropagation { get; set; } = false;

        public virtual void DrawCursor(SpriteBatch b) {
            if (Game1.options.hardwareCursor)
                return;
            b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
        }

        protected override void OnDraw(SpriteBatch b) {
            b.DrawMenuBox(this.AbsoluteBounds, this.GetGlobalDepth(0));
        }

        protected override bool OnKeyPressed(Keys key) => this.StopKeyPropagation;
    }
}

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ModUtilities.Menus.Components {
    public class MenuComponent : Component {
        public override Location ChildOrigin => new Location(base.ChildOrigin.X + Game1.tileSize, base.ChildOrigin.Y + Game1.tileSize);
        public virtual Rectangle ChildBounds => new Rectangle(this.ChildOrigin, new Size(this.Size.Width - 2 * Game1.tileSize, this.Size.Height - 2 * Game1.tileSize));
        public virtual bool StopKeyPropagation { get; set; } = false;

        protected virtual void DrawBackground(SpriteBatch b) {
            Rectangle bounds = this.AbsoluteBounds;
            Game1.drawDialogueBox(bounds.X + 0, bounds.Y - Game1.tileSize, bounds.Width + 0, bounds.Height + Game1.tileSize, false, true);
        }

        protected override void OnDraw(SpriteBatch b) {
            this.DrawBackground(b);
        }

        protected override bool OnKeyPressed(Keys key) => this.StopKeyPropagation;
    }
}

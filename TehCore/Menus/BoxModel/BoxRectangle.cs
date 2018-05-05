using Microsoft.Xna.Framework;

namespace TehCore.Menus.BoxModel {
    public struct BoxRectangle {
        public BoxVector Location { get; }
        public BoxVector Size { get; }

        public BoxRectangle(BoxVector location, BoxVector size) {
            this.Location = location;
            this.Size = size;
        }

        public Rectangle ToAbsolute(Vector2I parentSize) => this.ToAbsolute(parentSize.X, parentSize.Y);
        public Rectangle ToAbsolute(int parentWidth, int parentHeight) {
            Vector2I loc = this.Location.ToAbsolute(parentWidth, parentHeight);
            Vector2I size = this.Size.ToAbsolute(parentWidth, parentHeight);
            return new Rectangle(loc.X, loc.Y, size.X, size.Y);
        }
    }
}
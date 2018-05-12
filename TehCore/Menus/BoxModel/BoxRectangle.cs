namespace TehCore.Menus.BoxModel {
    public struct BoxRectangle {
        public BoxVector Location { get; }
        public BoxVector Size { get; }

        public BoxRectangle(BoxVector location, BoxVector size) {
            this.Location = location;
            this.Size = size;
        }

        public Rectangle2I ToAbsolute(Rectangle2I parentBounds) => this.ToAbsolute(parentBounds.Location.X, parentBounds.Location.Y, parentBounds.Size.X, parentBounds.Size.Y);
        public Rectangle2I ToAbsolute(int parentX, int parentY, int parentWidth, int parentHeight) {
            Vector2I loc = this.Location.ToAbsolute(parentWidth, parentHeight);
            Vector2I size = this.Size.ToAbsolute(parentWidth, parentHeight);
            return new Rectangle2I(loc.X + parentX, loc.Y + parentY, size.X, size.Y);
        }
    }
}
using Microsoft.Xna.Framework;

namespace ModUtilities.Menus {
    public struct RelativeRectangle {
        public RelativeLocation Location { get; }
        public RelativeSize Size { get; }

        public RelativeRectangle(RelativeLocation location, RelativeSize size) {
            this.Location = location;
            this.Size = size;
        }

        public static RelativeRectangle operator +(RelativeRectangle rect, RelativeLocation amount) {
            return new RelativeRectangle(rect.Location + amount, rect.Size);
        }

        public static RelativeRectangle operator -(RelativeRectangle rect, RelativeLocation amount) {
            return new RelativeRectangle(rect.Location - amount, rect.Size);
        }

        public Rectangle ToAbsolute(Rectangle parentBounds) {
            Point location = this.Location.ToAbsolute(parentBounds);
            Point size = this.Size.ToAbsolute(parentBounds);
            return new Rectangle(location.X, location.Y, size.X, size.Y);
        }
    }
}
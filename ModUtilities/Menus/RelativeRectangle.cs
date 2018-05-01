using System;
using Microsoft.Xna.Framework;

namespace ModUtilities.Menus {
    public struct RelativeRectangle : IEquatable<RelativeRectangle> {
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

        public static RelativeRectangle FromOffset(int offsetX, int offsetY, int offsetWidth, int offsetHeight) {
            return new RelativeRectangle(new RelativeLocation(0, 0, offsetX, offsetY), new RelativeSize(1f, 1f, offsetWidth, offsetHeight));
        }

        public static RelativeRectangle FromPercent(float percentX, float percentY, float percentWidth, float percentHeight) {
            return new RelativeRectangle(new RelativeLocation(percentX, percentY, 0, 0), new RelativeSize(percentWidth, percentHeight, 0, 0));
        }

        #region Equality
        public bool Equals(RelativeRectangle other) {
            return this.Location.Equals(other.Location) && this.Size.Equals(other.Size);
        }

        public override bool Equals(object obj) {
            if (object.ReferenceEquals(null, obj)) return false;
            return obj is RelativeRectangle rectangle && this.Equals(rectangle);
        }

        public override int GetHashCode() {
            unchecked {
                return (this.Location.GetHashCode() * 397) ^ this.Size.GetHashCode();
            }
        }
        #endregion
    }
}
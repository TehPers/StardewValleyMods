using System;
using Microsoft.Xna.Framework;

namespace ModUtilities.Menus {
    public struct RelativeSize : IEquatable<RelativeSize> {
        public float PercentWidth { get; }
        public float PercentHeight { get; }
        public int OffsetWidth { get; }
        public int OffsetHeight { get; }

        public RelativeSize(float percentWidth, float percentHeight, int offsetWidth, int offsetHeight) {
            this.PercentWidth = percentWidth;
            this.PercentHeight = percentHeight;
            this.OffsetWidth = offsetWidth;
            this.OffsetHeight = offsetHeight;
        }

        public RelativeSize Expand(RelativeSize amount) => this.Expand(amount.PercentWidth, amount.PercentHeight, amount.OffsetWidth, amount.OffsetHeight);
        public RelativeSize Expand(float percentWidth, float percentHeight, int offsetWidth, int offsetHeight) {
            return new RelativeSize(this.PercentWidth + percentWidth, this.PercentHeight + percentHeight, this.OffsetWidth + offsetWidth, this.OffsetHeight + offsetHeight);
        }

        public static RelativeSize operator +(RelativeSize a, RelativeSize b) {
            return a.Expand(b);
        }

        public static RelativeSize operator -(RelativeSize a, RelativeSize b) {
            return a.Expand(-b.PercentWidth, -b.PercentHeight, -b.OffsetWidth, -b.OffsetHeight);
        }

        public Point ToAbsolute(Rectangle parentBounds) {
            return new Point((int) (this.OffsetWidth + parentBounds.Width * this.PercentWidth), (int) (this.OffsetHeight + parentBounds.Height * this.PercentHeight));
        }

        #region Equality
        public bool Equals(RelativeSize other) {
            return this.PercentWidth.Equals(other.PercentWidth) && this.PercentHeight.Equals(other.PercentHeight) && this.OffsetWidth == other.OffsetWidth && this.OffsetHeight == other.OffsetHeight;
        }

        public override bool Equals(object obj) {
            if (object.ReferenceEquals(null, obj)) return false;
            return obj is RelativeSize size && this.Equals(size);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.PercentWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ this.PercentHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ this.OffsetWidth;
                hashCode = (hashCode * 397) ^ this.OffsetHeight;
                return hashCode;
            }
        }
        #endregion

        public static RelativeSize Zero { get; } = new RelativeSize(0, 0, 0, 0);
        public static RelativeSize Half { get; } = new RelativeSize(0.5f, 0.5f, 0, 0);
        public static RelativeSize Fill { get; } = new RelativeSize(1f, 1f, 0, 0);
    }
}
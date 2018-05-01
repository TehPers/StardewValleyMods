using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ModUtilities.Menus {
    public struct RelativeLocation : IEquatable<RelativeLocation> {
        public float PercentX { get; }
        public float PercentY { get; }
        public int OffsetX { get; }
        public int OffsetY { get; }

        public RelativeLocation(float percentX, float percentY, int offsetX, int offsetY) {
            this.PercentX = percentX;
            this.PercentY = percentY;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public RelativeLocation Translate(RelativeLocation amount) => this.Translate(amount.PercentX, amount.PercentY, amount.OffsetX, amount.OffsetY);
        public RelativeLocation Translate(float percentX, float percentY, int absoluteX, int absoluteY) {
            return new RelativeLocation(this.PercentX + percentX, this.PercentY + percentY, this.OffsetX + absoluteX, this.OffsetY + absoluteY);
        }

        public Point ToAbsolute(Rectangle parentBounds) {
            return new Point((int) (parentBounds.X + this.OffsetX + parentBounds.Width * this.PercentX), (int) (parentBounds.Y + this.OffsetY + parentBounds.Height * this.PercentY));
        }

        public static RelativeLocation operator +(RelativeLocation a, RelativeLocation b) {
            return a.Translate(b);
        }

        public static RelativeLocation operator -(RelativeLocation a, RelativeLocation b) {
            return a.Translate(-b);
        }

        public static RelativeLocation operator +(RelativeLocation a, RelativeSize b) {
            return a.Translate(b.PercentWidth, b.PercentHeight, b.OffsetWidth, b.OffsetHeight);
        }

        public static RelativeLocation operator -(RelativeLocation a, RelativeSize b) {
            return a.Translate(-b.PercentWidth, -b.PercentHeight, -b.OffsetWidth, -b.OffsetHeight);
        }

        public static RelativeLocation operator -(RelativeLocation location) {
            return new RelativeLocation(-location.PercentX, -location.PercentY, -location.OffsetX, -location.OffsetY);
        }

        #region Equality
        public bool Equals(RelativeLocation other) {
            return this.PercentX.Equals(other.PercentX) && this.PercentY.Equals(other.PercentY) && this.OffsetX == other.OffsetX && this.OffsetY == other.OffsetY;
        }

        public override bool Equals(object obj) {
            if (object.ReferenceEquals(null, obj)) return false;
            return obj is RelativeLocation location && this.Equals(location);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.PercentX.GetHashCode();
                hashCode = (hashCode * 397) ^ this.PercentY.GetHashCode();
                hashCode = (hashCode * 397) ^ this.OffsetX;
                hashCode = (hashCode * 397) ^ this.OffsetY;
                return hashCode;
            }
        }
        #endregion

        public static RelativeLocation TopLeft { get; } = new RelativeLocation(0, 0, 0, 0);
        public static RelativeLocation TopMiddle { get; } = new RelativeLocation(0.5F, 0, 0, 0);
        public static RelativeLocation TopRight { get; } = new RelativeLocation(1F, 0, 0, 0);
        public static RelativeLocation MiddleLeft { get; } = new RelativeLocation(0, 0.5F, 0, 0);
        public static RelativeLocation Center { get; } = new RelativeLocation(0.5F, 0.5F, 0, 0);
        public static RelativeLocation MiddleRight { get; } = new RelativeLocation(1F, 0.5F, 0, 0);
        public static RelativeLocation BottomLeft { get; } = new RelativeLocation(0, 1F, 0, 0);
        public static RelativeLocation BottomMiddle { get; } = new RelativeLocation(0.5F, 1F, 0, 0);
        public static RelativeLocation BottomRight { get; } = new RelativeLocation(1F, 1F, 0, 0);
    }
}

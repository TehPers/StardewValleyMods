using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ModUtilities.Menus {
    public struct RelativeLocation {
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

        public RelativeLocation Translate(int absoluteX, int absoluteY) {
            return new RelativeLocation(this.PercentX, this.PercentY, this.OffsetX + absoluteX, this.OffsetY + absoluteY);
        }

        public Point ToAbsolute(Rectangle parentBounds) {
            return new Point((int) (parentBounds.X + this.OffsetX + parentBounds.Width * this.PercentX), (int) (parentBounds.Y + this.OffsetY + parentBounds.Height * this.PercentY));
        }

        public static RelativeLocation operator +(RelativeLocation a, RelativeLocation b) {
            return new RelativeLocation(a.PercentX + b.PercentX, a.PercentY + b.PercentY, a.OffsetX + b.OffsetX, a.OffsetY + b.OffsetY);
        }

        public static RelativeLocation operator -(RelativeLocation a, RelativeLocation b) {
            return new RelativeLocation(a.PercentX - b.PercentX, a.PercentY - b.PercentY, a.OffsetX - b.OffsetX, a.OffsetY - b.OffsetY);
        }

        public static RelativeLocation operator +(RelativeLocation a, RelativeSize b) {
            return new RelativeLocation(a.PercentX + b.PercentWidth, a.PercentY + b.PercentHeight, a.OffsetX + b.OffsetWidth, a.OffsetY + b.OffsetHeight);
        }

        public static RelativeLocation operator -(RelativeLocation a, RelativeSize b) {
            return new RelativeLocation(a.PercentX - b.PercentWidth, a.PercentY - b.PercentHeight, a.OffsetX - b.OffsetWidth, a.OffsetY - b.OffsetHeight);
        }

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

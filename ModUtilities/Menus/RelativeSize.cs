using Microsoft.Xna.Framework;

namespace ModUtilities.Menus {
    public struct RelativeSize {
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

        public Point ToAbsolute(Rectangle parentBounds) {
            return new Point((int) (this.OffsetWidth + parentBounds.Width * this.PercentWidth), (int) (this.OffsetHeight + parentBounds.Height * this.PercentHeight));
        }
    }
}
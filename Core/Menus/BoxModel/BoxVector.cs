namespace TehPers.Core.Menus.BoxModel {
    public struct BoxVector {
        public static BoxVector Zero { get; } = new BoxVector(0, 0, 0, 0);
        public static BoxVector Fill { get; } = new BoxVector(0, 0, 1, 1);

        public int AbsoluteX { get; }
        public int AbsoluteY { get; }
        public float PercentX { get; }
        public float PercentY { get; }

        public BoxVector(int absoluteX, int absoluteY, float percentX, float percentY) {
            this.AbsoluteX = absoluteX;
            this.AbsoluteY = absoluteY;
            this.PercentX = percentX;
            this.PercentY = percentY;
        }

        public Vector2I ToAbsolute(Vector2I parentSize) => this.ToAbsolute(parentSize.X, parentSize.Y);
        public Vector2I ToAbsolute(int parentWidth, int parentHeight) {
            return new Vector2I((int) (this.PercentX * parentWidth) + this.AbsoluteX, (int) (this.PercentY * parentHeight) + this.AbsoluteY);
        }
    }
}
namespace TehCore.Menus.BoxModel {
    public struct OuterSize {
        public static OuterSize Zero { get; } = new OuterSize(0);

        /// <summary>Absolute size on the left</summary>
        public int Left { get; }

        /// <summary>Absolute size on the right</summary>
        public int Right { get; }

        /// <summary>Absolute size on the top</summary>
        public int Top { get; }

        /// <summary>Absolute size on the bottom</summary>
        public int Bottom { get; }

        public OuterSize(int left, int right, int top, int bottom) {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public OuterSize(int leftRight, int topBottom) : this(leftRight, leftRight, topBottom, topBottom) { }

        public OuterSize(int size) : this(size, size, size, size) { }
        
        public OuterSize(OuterSize source, int? left = null, int? right = null, int? top = null, int? bottom = null) : this(left ?? source.Left, right ?? source.Right, top ?? source.Top, bottom ?? source.Bottom) { }
    }
}

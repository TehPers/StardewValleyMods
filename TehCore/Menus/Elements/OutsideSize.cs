namespace TehCore.Menus.Elements {
    public struct OutsideSize {
        /// <summary>Pixels to the top</summary>
        public int Top { get; }
        public int Bottom { get; }
        public int Left { get; }
        public int Right { get; }

        public OutsideSize(int top, int bottom, int left, int right) {
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
        }

        public static OutsideSize Zero { get; } = new OutsideSize(0, 0, 0, 0);
    }
}
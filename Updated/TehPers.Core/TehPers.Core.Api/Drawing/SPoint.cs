using System;
using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Drawing
{
    public readonly struct SPoint : IEquatable<SPoint>, IEquatable<Point>
    {
        public int X { get; }

        public int Y { get; }

        public SPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = this.X;
            y = this.Y;
        }

        public bool Equals(SPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public bool Equals(Point other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                SPoint p => this.Equals(p),
                Point p => this.Equals(p),
                _ => false,
            };
        }

        public override int GetHashCode()
        {
            return unchecked((this.X * 397) ^ this.Y);
        }

        public override string ToString()
        {
            return $"{{{{X:{this.X} Y:{this.Y}}}}}";
        }

        public static bool operator ==(in SPoint first, in SPoint second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(in SPoint first, in SPoint second)
        {
            return !first.Equals(second);
        }

        public static SPoint Zero => new SPoint(0, 0);
    }
}
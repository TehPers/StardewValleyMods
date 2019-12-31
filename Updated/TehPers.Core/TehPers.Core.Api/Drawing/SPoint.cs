using System;
using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Drawing
{
    /// <summary>
    /// A readonly 2-dimensional point with implicit conversions to and from <see cref="Point"/>.
    /// </summary>
    public readonly struct SPoint : IEquatable<SPoint>, IEquatable<Point>
    {
        /// <summary>
        /// Gets (0, 0).
        /// </summary>
        public static SPoint Zero => new SPoint(0, 0);

        /// <summary>
        /// Checks for equality between two points.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>Whether the two points are equal.</returns>
        public static bool operator ==(in SPoint first, in SPoint second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Checks for inequality between two points.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>Whether the two points are not equal.</returns>
        public static bool operator !=(in SPoint first, in SPoint second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Gets the x-coordinate of the point.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the y-coordinate of the point.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SPoint"/> struct.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        public SPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Deconstructs this point into its components.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        public void Deconstruct(out int x, out int y)
        {
            x = this.X;
            y = this.Y;
        }

        /// <inheritdoc />
        public bool Equals(SPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <inheritdoc />
        public bool Equals(Point other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj switch
            {
                SPoint p => this.Equals(p),
                Point p => this.Equals(p),
                _ => false,
            };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return unchecked((this.X * 397) ^ this.Y);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{{{X:{this.X} Y:{this.Y}}}}}";
        }
    }
}
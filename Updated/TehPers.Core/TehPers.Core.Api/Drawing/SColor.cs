using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace TehPers.Core.Api.Drawing
{
    /// <summary>
    /// A 4-byte representation of a color.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct SColor : IEquatable<SColor>, IEquatable<Color>
    {
        /// <summary>
        /// Implicitly converts a <see cref="SColor"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="source">The source <see cref="SColor"/>.</param>
        /// <returns>The converted <see cref="Color"/>.</returns>
        public static implicit operator Color(in SColor source)
        {
            return new Color(source.R, source.G, source.B, source.A);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Color"/> to a <see cref="SColor"/>.
        /// </summary>
        /// <param name="source">The source <see cref="Color"/>.</param>
        /// <returns>The converted <see cref="SColor"/>.</returns>
        public static implicit operator SColor(Color source)
        {
            return new SColor(source.R, source.G, source.B, source.A);
        }

        /// <summary>
        /// Adds two <see cref="SColor"/>s by adding each individual component. If an operation would overflow, the result is <see cref="byte.MaxValue"/>.
        /// </summary>
        /// <param name="first">The first <see cref="SColor"/> (LHS).</param>
        /// <param name="second">The second <see cref="SColor"/> (RHS).</param>
        /// <returns>The added colors.</returns>
        public static SColor operator +(in SColor first, in SColor second)
        {
            var r = (byte)Math.Min(byte.MaxValue, first.R + second.R);
            var g = (byte)Math.Min(byte.MaxValue, first.G + second.G);
            var b = (byte)Math.Min(byte.MaxValue, first.B + second.B);
            var a = (byte)Math.Min(byte.MaxValue, first.A + second.A);
            return new SColor(r, g, b, a);
        }

        /// <summary>
        /// Subtracts two <see cref="SColor"/>s by subtracting each individual component. If an operation would underflow, the result is <see cref="byte.MinValue"/>.
        /// </summary>
        /// <param name="first">The first <see cref="SColor"/> (LHS).</param>
        /// <param name="second">The second <see cref="SColor"/> (RHS).</param>
        /// <returns>The subtracted colors.</returns>
        public static SColor operator -(in SColor first, in SColor second)
        {
            var r = (byte)Math.Max(byte.MinValue, first.R - second.R);
            var g = (byte)Math.Max(byte.MinValue, first.G - second.G);
            var b = (byte)Math.Max(byte.MinValue, first.B - second.B);
            var a = (byte)Math.Max(byte.MinValue, first.A - second.A);
            return new SColor(r, g, b, a);
        }

        /// <summary>
        /// Multiplies each individual component of a <see cref="SColor"/> by a scalar. The operation is clamped to [<see cref="byte.MinValue"/>, <see cref="byte.MaxValue"/>].
        /// </summary>
        /// <param name="first">The <see cref="SColor"/> to multiply.</param>
        /// <param name="second">The scalar.</param>
        /// <returns>The mulitplied color.</returns>
        public static SColor operator *(in SColor first, in float second)
        {
            var r = (byte)Math.Min(Math.Max(first.R * second, byte.MinValue), byte.MaxValue);
            var g = (byte)Math.Min(Math.Max(first.G * second, byte.MinValue), byte.MaxValue);
            var b = (byte)Math.Min(Math.Max(first.B * second, byte.MinValue), byte.MaxValue);
            var a = (byte)Math.Min(Math.Max(first.A * second, byte.MinValue), byte.MaxValue);
            return new SColor(r, g, b, a);
        }

        /// <summary>
        /// Multiplies each individual component of a <see cref="SColor"/> by a scalar. The operation is clamped to [<see cref="byte.MinValue"/>, <see cref="byte.MaxValue"/>].
        /// </summary>
        /// <param name="first">The scalar.</param>
        /// <param name="second">The <see cref="SColor"/> to multiply.</param>
        /// <returns>The mulitplied color.</returns>
        public static SColor operator *(in float first, in SColor second)
        {
            return second * first;
        }

        /// <summary>
        /// Divides each individual component of a <see cref="SColor"/> by a scalar. The operation is clamped to [<see cref="byte.MinValue"/>, <see cref="byte.MaxValue"/>].
        /// </summary>
        /// <param name="first">The <see cref="SColor"/> to divide.</param>
        /// <param name="second">The scalar.</param>
        /// <returns>The divided color.</returns>
        public static SColor operator /(in SColor first, in float second)
        {
            return first * (1F / second);
        }

        /// <summary>
        /// Compares two colors for equality.
        /// </summary>
        /// <param name="first">The first color.</param>
        /// <param name="second">The second color.</param>
        /// <returns>A value indicating whether the two colors were equal.</returns>
        public static bool operator ==(in SColor first, in SColor second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Compares two colors for equality.
        /// </summary>
        /// <param name="first">The first color.</param>
        /// <param name="second">The second color.</param>
        /// <returns>A value indicating whether the two colors were equal.</returns>
        public static bool operator ==(in SColor first, Color second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Compares two colors for inequality.
        /// </summary>
        /// <param name="first">The first color.</param>
        /// <param name="second">The second color.</param>
        /// <returns>A value indicating whether the two colors were not equal.</returns>
        public static bool operator !=(in SColor first, in SColor second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Compares two colors for inequality.
        /// </summary>
        /// <param name="first">The first color.</param>
        /// <param name="second">The second color.</param>
        /// <returns>A value indicating whether the two colors were not equal.</returns>
        public static bool operator !=(in SColor first, Color second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Gets the 4-byte packed value of the color. Layout order (from high byte to low byte) is A, B, G, R.
        /// </summary>
        [field: FieldOffset(0)]
        public uint PackedValue { get; }

        /// <summary>
        /// Gets the alpha component.
        /// </summary>
        [field: FieldOffset(3)]
        public byte A { get; }

        /// <summary>
        /// Gets the red component.
        /// </summary>
        [field: FieldOffset(0)]
        public byte R { get; }

        /// <summary>
        /// Gets the green component.
        /// </summary>
        [field: FieldOffset(1)]
        public byte G { get; }

        /// <summary>
        /// Gets the blue component.
        /// </summary>
        [field: FieldOffset(2)]
        public byte B { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SColor"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public SColor(float r, float g, float b, float a = 1F)
            : this((byte)(r * byte.MaxValue), (byte)(g * byte.MaxValue), (byte)(b * byte.MaxValue), (byte)(a * byte.MaxValue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SColor"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public SColor(byte r, byte g, byte b, byte a = 255)
        {
            this.PackedValue = default;
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SColor"/> struct.
        /// </summary>
        /// <param name="packedValue">The 4-byte packed value of the color.</param>
        public SColor(uint packedValue)
        {
            this.A = default;
            this.R = default;
            this.G = default;
            this.B = default;
            this.PackedValue = packedValue;
        }

        /// <inheritdoc/>
        public bool Equals(SColor other)
        {
            return this.PackedValue == other.PackedValue;
        }

        /// <inheritdoc />
        public bool Equals(Color other)
        {
            return this.A == other.A && this.R == other.R && this.G == other.G && this.B == other.B;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj switch
            {
                SColor other => this.Equals(other),
                Color other => this.Equals(other),
                _ => false,
            };
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int)this.PackedValue;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{{{R:{this.R} G:{this.G} B:{this.B} A:{this.A}}}}}";
        }
    }
}

using Microsoft.Xna.Framework;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct SColor {
        public uint PackedValue { get; }
        public byte A => unchecked((byte) (this.PackedValue >> 24));
        public byte R => unchecked((byte) (this.PackedValue >> 16));
        public byte G => unchecked((byte) (this.PackedValue >> 8));
        public byte B => unchecked((byte) this.PackedValue);

        public SColor(float r, float g, float b) : this(r, g, b, 0) { }
        public SColor(float r, float g, float b, float a) : this((byte) (r * byte.MaxValue), (byte) (g * byte.MaxValue), (byte) (b * byte.MaxValue), (byte) (a * byte.MaxValue)) { }
        public SColor(byte r, byte g, byte b) : this(r, g, b, 1) { }
        public SColor(byte r, byte g, byte b, byte a) : this((uint) (a << 24) + (uint) (r << 16) + (uint) (g << 8) + b) { }
        public SColor(uint packedValue) {
            this.PackedValue = packedValue;
        }

        public static implicit operator Color(in SColor source) {
            return new Color(source.R, source.G, source.B, source.A);
        }

        public static implicit operator SColor(Color source) {
            return new SColor(source.R, source.G, source.B, source.A);
        }
    }
}

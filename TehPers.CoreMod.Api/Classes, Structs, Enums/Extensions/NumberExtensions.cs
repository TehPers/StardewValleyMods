using System;
using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Extensions {
    public static class NumberExtensions {
        /// <summary>Converts the number into bytes from lowest order byte to highest order byte.</summary>
        /// <param name="source">The number to convert.</param>
        /// <returns>The bytes in the number, from lowest to highest order.</returns>
        public static IEnumerable<byte> GetBytes(this int source) {
            const int bits = 32;
            const int bitsPerByte = 4;
            for (int i = 0; i < bits / bitsPerByte; i++) {
                yield return (byte) source;
                source >>= bitsPerByte;
            }
        }

        /// <summary>Converts the number into bytes from lowest order byte to highest order byte.</summary>
        /// <param name="source">The number to convert.</param>
        /// <returns>The bytes in the number, from lowest to highest order.</returns>
        public static IEnumerable<byte> GetBytes(this uint source) {
            const int bits = 32;
            const int bitsPerByte = 4;
            for (int i = 0; i < bits / bitsPerByte; i++) {
                yield return (byte) source;
                source >>= bitsPerByte;
            }
        }

        #region Clamp
        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static sbyte Clamp(this sbyte value, sbyte lower, sbyte upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static byte Clamp(this byte value, byte lower, byte upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static ushort Clamp(this ushort value, ushort lower, ushort upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static short Clamp(this short value, short lower, short upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static uint Clamp(this uint value, uint lower, uint upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static int Clamp(this int value, int lower, int upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static ulong Clamp(this ulong value, ulong lower, ulong upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static long Clamp(this long value, long lower, long upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static float Clamp(this float value, float lower, float upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static double Clamp(this double value, double lower, double upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }

        /// <summary>Clamps a value between a lower and upper bound, inclusive.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lower">The lower bound, inclusive.</param>
        /// <param name="upper">The upper bound, inclusive.</param>
        /// <returns>The number closest to <paramref name="value"/> which is in the given bounds.</returns>
        public static decimal Clamp(this decimal value, decimal lower, decimal upper) {
            return Math.Max(Math.Min(value, upper), lower);
        }
        #endregion
    }
}
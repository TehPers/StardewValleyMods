// This file is heavily based on (but not a direct copy of) the source code for .NET Core's libraries, by the .NET Foundation and Contributors.
// The original source code is available on GitHub at https://github.com/dotnet/corefx.
// The source code is licensed under the MIT license: https://github.com/dotnet/corefx/blob/master/LICENSE.TXT.

using System.Collections;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System {
    /// <summary>Provides static methods for generating tuples.</summary>
    [Serializable]
    public partial struct ValueTuple : IEquatable<ValueTuple>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple> {
        internal static int CombineHashes(params object[] items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            return items.Aggregate(0, (current, item) => unchecked((current * 397) ^ (item?.GetHashCode() ?? 0)));
        }

        /// <inheritdoc />
        public int CompareTo(object other, IComparer comparer) {
            return other is ValueTuple ? 0 : throw new ArgumentException();
        }

        /// <inheritdoc />
        public int CompareTo(object obj) {
            return obj is ValueTuple ? 0 : throw new ArgumentException();
        }

        /// <inheritdoc />
        public int CompareTo(ValueTuple other) {
            return 0;
        }

        /// <inheritdoc />
        public bool Equals(ValueTuple other) {
            return true;
        }

        /// <inheritdoc />
        public bool Equals(object other, IEqualityComparer comparer) {
            return other is ValueTuple;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is ValueTuple;
        }

        /// <inheritdoc />
        public int GetHashCode(IEqualityComparer comparer) {
            return 0;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return 0;
        }
    }
}

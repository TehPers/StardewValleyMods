using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace TehPers.Core.Api.Content
{
    /// <summary>
    /// A location for a resource.
    /// </summary>
    [Obsolete]
    public readonly struct AssetPath : IEquatable<AssetPath>
    {
        public static IEnumerable<string> GetParts(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(ImmutableStack<string>.Empty, (parts, part) =>
                {
                    return part switch
                    {
                        "." => parts, // Do nothing
                        ".." when !parts.Any() => parts.Push(part), // Move outside the root/current directory
                        ".." when parts.Peek() == ".." => parts.Push(part), // Move even further outside the root/current directory
                        ".." => parts.Pop(), // Move up a directory
                        _ => parts.Push(part), // Move into a directory
                    };
                })
                .Reverse()
                .AsEnumerable();
        }

        public static string Normalize(string path)
        {
            return string.Join("\\", AssetPath.GetParts(path));
        }

        public static bool operator ==(AssetPath left, AssetPath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetPath left, AssetPath right)
        {
            return !(left == right);
        }

        private readonly string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetPath"/> struct.
        /// </summary>
        /// <param name="path">The relative path to the asset.</param>
        public AssetPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            }

            this.path = AssetPath.Normalize(path);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is AssetPath other && this.Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(AssetPath other)
        {
            return string.Equals(this.path, other.path, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.path;
        }
    }
}